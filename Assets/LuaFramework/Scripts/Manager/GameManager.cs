using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;
using System.Reflection;
using System.IO;
using BestHTTP;
using LitJson;
namespace LuaFramework {
    public class GameManager : Manager {
        protected static bool initialize = false;
        private List<string> downloadFiles = new List<string>();

        /// <summary>
        /// 初始化游戏管理器
        /// </summary>
        void Awake() {
            Init();
        }
static GameObject go;
        /// <summary>
        /// 初始化
        /// </summary>
       public void Init() {
         //  GameManager.GetOrCreate(gameObject);
            DontDestroyOnLoad(gameObject==null?go:gameObject);  //防止销毁自己
            if (AppConst.ForceUpdateMode)
            {
                CheckForceUpdata();
            }else{
                CheckExtractResource(); //释放资源
            }
           
          
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.targetFrameRate = AppConst.GameFrameRate;
        }


    public void AddComponent(){
        if (go==null)
       {
              go = GameObject.Find("GameManager");
            go.AddComponent<GameManager>();
       }
    }
        /// <summary>
        /// 检查强制更新
        /// </summary>
        public void CheckForceUpdata(){
             facade.SendMessageCommand(NotiConst.UPDATE_FORCE_CHECK, "检查强制更新");
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {   
                CheckForceUpdataFail();
                return;
            }else{

            }
            string url = AppConst.ForceUpdataUrl;
            
          //测试
          //  string url = "http://test.majiang.esgame.com/Version/VersionUpdate?version=1.1&Com_PT=2";
            
             HTTPRequest httpRequest = new HTTPRequest(new Uri(url),
         HTTPMethods.Get,
        (req, response) => {
			int code = 0;
			string message = "";
			string dataString = "";

            if (req.Exception != null){

				message = req.Exception.ToString();
                Debug.Log("Exception = "+message);
				CheckForceUpdataFail();
            }
            else {
                try {
                    if (response!=null) {
                        Debug.Log("请求的Url: "+url+"=====>"+response.DataAsText);
                    	JsonData jsonData = JsonMapper.ToObject(response.DataAsText);

                    	code = int.Parse(jsonData["ResultCode"].ToString());
                        if (code==null)
                        {
                            Debug.Log("Code非int类型");
                        }
						message = jsonData["ResultMessage"].ToString();

						JsonData data = jsonData["Data"];
						
                    

					
					Debug.Log("code： " + code);
					Debug.Log("message " + message);
					
                    if (code == -2) //无强制更新
                    {   
                        facade.SendMessageCommand(NotiConst.UPDATE_FORCE_NONE, "");
                        CheckExtractResource(); //释放资源
                    }else if (code == 1) //有强制更新
                    {   
                        
						if(data != null) {
							dataString = JsonMapper.ToJson(data);
                            Debug.Log("dataString " + dataString);

                           string DownUrl = data["DownUrl"].ToString();
                           Debug.Log(DownUrl);
                             facade.SendMessageCommand(NotiConst.UPDATE_FORCE, DownUrl);
						}
                    }

                    }else{
                        CheckForceUpdataFail();
                    }
					
                }
                catch (Exception ex) {
                    Debug.Log(ex);
					message = ex.ToString();
					CheckForceUpdataFail();
                }
            }
        });

        //测试注释掉
        WWWForm parameter=new WWWForm();
		parameter.AddField("version", AppConst.Product_Version);
		#if UNITY_ANDROID
        
			 parameter.AddField("Com_PT", 2);
		#elif UNITY_IPHONE
			 parameter.AddField("Com_PT", 1);
             #elif UNITY_EDITOR
             parameter.AddField("Com_PT", 2);
            
		#endif
        httpRequest.SetFields(parameter);

        httpRequest.Send();

            
        }

         /// <summary>
        /// 检查强制更新
        /// </summary>
        public void CheckForceUpdataFail(){
            facade.SendMessageCommand(NotiConst.UPDATE_FORCE_CHECKFAIL, "");
            
        }
 public GameManager()
    {
         
    }

    
    static public GameManager GetOrCreate(GameObject gameObject) {
        if (gameObject == null) { 
            
            return new GameManager(); }
        var existed = gameObject.GetComponent <GameManager>();
        return existed ?? gameObject.AddComponent <GameManager>();
    }


        /// <summary>
        /// 释放资源
        /// </summary>
        public void CheckExtractResource() {
            bool isExists = Directory.Exists(Util.DataPath) &&
              Directory.Exists(Util.DataPath + "lua/") && File.Exists(Util.DataPath + "files.txt");
            if (isExists || AppConst.DebugMode) {
                StartCoroutine(GameManager.GetOrCreate(gameObject).OnUpdateResource());
                return;   //文件已经解压过了，自己可添加检查文件列表逻辑
            }
            StartCoroutine(GameManager.GetOrCreate(gameObject).OnExtractResource());    //启动释放协成 
        }

        IEnumerator OnExtractResource() {
            string dataPath = Util.DataPath;  //数据目录
            string resPath = Util.AppContentPath(); //游戏包资源目录

            if (Directory.Exists(dataPath)) Directory.Delete(dataPath, true);
            Directory.CreateDirectory(dataPath);

            string infile = resPath + "files.txt";
            string outfile = dataPath + "files.txt";
            if (File.Exists(outfile)) File.Delete(outfile);

            string message = "正在解包文件:>files.txt";
            Debug.Log(infile);
            Debug.Log(outfile);
            if (Application.platform == RuntimePlatform.Android) {
                WWW www = new WWW(infile);
                yield return www;

                if (www.isDone) {
                    File.WriteAllBytes(outfile, www.bytes);
                }
                yield return 0;
            } else File.Copy(infile, outfile, true);
            yield return new WaitForEndOfFrame();

            //释放所有文件到数据目录
            string[] files = File.ReadAllLines(outfile);
            int totalCount = files.Length;
            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
           
                string[] fs = file.Split('|');
                infile = resPath + fs[0];  //
                outfile = dataPath + fs[0];
                double progresValue= (double)i/totalCount;
                message = "正在解包文件"+Math.Round(progresValue*100,2);
                Debug.Log(message);
                facade.SendMessageCommand(NotiConst.UPDATE_MESSAGE, message);

                string dir = Path.GetDirectoryName(outfile);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                if (Application.platform == RuntimePlatform.Android) {
                    WWW www = new WWW(infile);
                    yield return www;

                    if (www.isDone) {
                        File.WriteAllBytes(outfile, www.bytes);
                    }
                    yield return 0;
                } else {
                    if (File.Exists(outfile)) {
                        File.Delete(outfile);
                    }
                    File.Copy(infile, outfile, true);
                }
                yield return new WaitForEndOfFrame();
            }
            message = "解包完成!";
            facade.SendMessageCommand(NotiConst.UPDATE_MESSAGE, message);
            yield return new WaitForSeconds(0.1f);

            message = string.Empty;
            //释放完成，开始启动更新资源
            StartCoroutine(OnUpdateResource());
        }

        /// <summary>
        /// 启动更新下载，这里只是个思路演示，此处可启动线程下载更新
        /// </summary>
        IEnumerator OnUpdateResource() {
            if (!AppConst.UpdateMode) {
                OnResourceInited();
                yield break;
            }
            string dataPath = Util.DataPath;  //数据目录
            string url = AppConst.WebUrl;
            string message = string.Empty;
            string random = DateTime.Now.ToString("yyyymmddhhmmss");
            string listUrl = url + "files.txt?v=" + random;
            Debug.LogWarning("LoadUpdate---->>>" + listUrl);

            WWW www = new WWW(listUrl); yield return www;
            if (www.error != null) {
                OnUpdateFailed(string.Empty);
                yield break;
            }
            if (!Directory.Exists(dataPath)) {
                Directory.CreateDirectory(dataPath);
            }
            File.WriteAllBytes(dataPath + "files.txt", www.bytes);
            string filesText = www.text;
            string[] files = filesText.Split('\n');
            int totalCount2 = files.Length;
            for (int i = 0; i < files.Length; i++) {
                if (string.IsNullOrEmpty(files[i])) continue;
                string[] keyValue = files[i].Split('|');
                string f = keyValue[0];
                string localfile = (dataPath + f).Trim();
                string path = Path.GetDirectoryName(localfile);
                if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }
                string fileUrl = url + f + "?v=" + random;
                bool canUpdate = !File.Exists(localfile);
                if (!canUpdate) {
                    string remoteMd5 = keyValue[1].Trim();
                    string localMd5 = Util.md5file(localfile);
                    canUpdate = !remoteMd5.Equals(localMd5);
                    if (canUpdate) File.Delete(localfile);
                }
                if (canUpdate) {   //本地缺少文件
                    Debug.Log(fileUrl);
                    double progressValue2 = (double)i/totalCount2;
                    
                    message = "正在下载更新文件"+Math.Round(progressValue2*100,2);
                    Debug.Log(message);
                    facade.SendMessageCommand(NotiConst.UPDATE_MESSAGE, message);
                    /*
                    www = new WWW(fileUrl); yield return www;
                    if (www.error != null) {
                        OnUpdateFailed(path);   //
                        yield break;
                    }
                    File.WriteAllBytes(localfile, www.bytes);
                     */
                    //这里都是资源文件，用线程下载
                    BeginDownload(fileUrl, localfile);
                    while (!(IsDownOK(localfile))) { yield return new WaitForEndOfFrame(); }
                }
            }
            yield return new WaitForEndOfFrame();

            message = "更新完成!";
            facade.SendMessageCommand(NotiConst.UPDATE_MESSAGE, message);

            OnResourceInited();
        }

        void OnUpdateFailed(string file) {
            string message = "更新失败!";
            facade.SendMessageCommand(NotiConst.UPDATE_MESSAGE, message);
        }

        /// <summary>
        /// 是否下载完成
        /// </summary>
        bool IsDownOK(string file) {
            return downloadFiles.Contains(file);
        }

        /// <summary>
        /// 线程下载
        /// </summary>
        void BeginDownload(string url, string file) {     //线程下载
            object[] param = new object[2] { url, file };

            ThreadEvent ev = new ThreadEvent();
            ev.Key = NotiConst.UPDATE_DOWNLOAD;
            ev.evParams.AddRange(param);
            ThreadManager.AddEvent(ev, OnThreadCompleted);   //线程下载
        }

        /// <summary>
        /// 线程完成
        /// </summary>
        /// <param name="data"></param>
        void OnThreadCompleted(NotiData data) {
            switch (data.evName) {
                case NotiConst.UPDATE_EXTRACT:  //解压一个完成
                //
                break;
                case NotiConst.UPDATE_DOWNLOAD: //下载一个完成
                downloadFiles.Add(data.evParam.ToString());
                break;
            }
        }

        /// <summary>
        /// 资源初始化结束
        /// </summary>
        public void OnResourceInited() {
#if ASYNC_MODE
            ResManager.Initialize(AppConst.AssetDir, delegate() {
                Debug.Log("Initialize OK!!!");
                this.OnInitialize();
            });
#else
            ResManager.Initialize();
            this.OnInitialize();
#endif
        }

        void OnInitialize() {
            LuaManager.InitStart();
            LuaManager.DoFile("Logic/Game");         //加载游戏
            LuaManager.DoFile("Logic/Network");      //加载网络
            LuaManager.DoFile("Logic/Network2");      //加载网络
            NetManager.OnInit();                     //初始化网络
            NetManager2.OnInit();                     //初始化网络
            Util.CallMethod("Game", "OnInitOK");     //初始化完成

            initialize = true;

            // //类对象池测试
            // var classObjPool = ObjPoolManager.CreatePool<TestObjectClass>(OnPoolGetElement, OnPoolPushElement);
            // //方法1
            // //objPool.Release(new TestObjectClass("abcd", 100, 200f));
            // //var testObj1 = objPool.Get();

            // //方法2
            // ObjPoolManager.Release<TestObjectClass>(new TestObjectClass("abcd", 100, 200f));
            // var testObj1 = ObjPoolManager.Get<TestObjectClass>();

            // Debugger.Log("TestObjectClass--->>>" + testObj1.ToString());

            // //游戏对象池测试
            // var prefab = Resources.Load("TestGameObjectPrefab", typeof(GameObject)) as GameObject;
            // var gameObjPool = ObjPoolManager.CreatePool("TestGameObject", 5, 10, prefab);

            // var gameObj = Instantiate(prefab) as GameObject;
            // gameObj.name = "TestGameObject_01";
            // gameObj.transform.localScale = Vector3.one;
            // gameObj.transform.localPosition = Vector3.zero;

            // ObjPoolManager.Release("TestGameObject", gameObj);
            // var backObj = ObjPoolManager.Get("TestGameObject");
            // backObj.transform.SetParent(null);

            // Debug.Log("TestGameObject--->>>" + backObj);
        }

        /// <summary>
        /// 当从池子里面获取时
        /// </summary>
        /// <param name="obj"></param>
        void OnPoolGetElement(TestObjectClass obj) {
            Debug.Log("OnPoolGetElement--->>>" + obj);
        }

        /// <summary>
        /// 当放回池子里面时
        /// </summary>
        /// <param name="obj"></param>
        void OnPoolPushElement(TestObjectClass obj) {
            Debug.Log("OnPoolPushElement--->>>" + obj);
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        void OnDestroy() {
            if (NetManager != null) {
                NetManager.Unload();
            }
            if (LuaManager != null) {
                LuaManager.Close();
            }
            Debug.Log("~GameManager was destroyed");
        }
    }
}