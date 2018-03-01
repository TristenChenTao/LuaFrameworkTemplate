using System.Collections.Generic;
using FairyGUI;
using LuaFramework;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class AppView : View {
    private string message = "";

    private GTextField updateDetail;
    GProgressBar pb;
    GComponent view;
    GameManager gameManager;
    GComponent failView;
    
    ///<summary>
    /// 监听的消息
    ///</summary>
    List<string> MessageList {
        get {
            return new List<string> () {
                NotiConst.UPDATE_MESSAGE,
                    NotiConst.UPDATE_EXTRACT,
                    NotiConst.UPDATE_DOWNLOAD,
                    NotiConst.UPDATE_PROGRESS,
                    NotiConst.UPDATE_FORCE_CHECK,
                    NotiConst.UPDATE_FORCE,
                    NotiConst.UPDATE_FORCE_CHECKFAIL,
                    NotiConst.UPDATE_FORCE_NONE,
            };
        }
    }

    void Awake () {
        RemoveMessage (this, MessageList);
        RegisterMessage (this, MessageList);
        gameManager  = new GameManager();

        GRoot.inst.SetContentScaleFactor (1920, 1080,UIContentScaler.ScreenMatchMode.MatchWidth);
        UIPackage.AddPackage ("UI/login");
        view = UIPackage.CreateObject ("login", "AppViewBg").asCom;
        view.SetSize (GRoot.inst.width, GRoot.inst.height);
        view.AddRelation (GRoot.inst, RelationType.Size);
        GRoot.inst.AddChild (view);

        //版本号
        // this.updateDetail = view.GetChild ("n15").asTextField;
    }

    /// <summary>
    /// 处理View消息
    /// </summary>
    /// <param name="message"></param>
    public override void OnMessage (IMessage message) {
        string name = message.Name;
        object body = message.Body;
        Debug.Log(name);
        switch (name) {
            case NotiConst.UPDATE_MESSAGE: //更新消息
                UpdateMessage (body.ToString ());
                break;
            case NotiConst.UPDATE_EXTRACT: //更新解压
                //UpdateExtract ("解压中");
                break;
            case NotiConst.UPDATE_DOWNLOAD: //更新下载
               // UpdateDownload ("下载文件");
                break;
            case NotiConst.UPDATE_PROGRESS: //更新下载进度
               // UpdateProgress ("下载中");
                break;
            case NotiConst.UPDATE_FORCE_CHECK: //强制更新检查
                ForceUpdataCheckUI();
                break;
            case NotiConst.UPDATE_FORCE: //强制更新
                 ForceUpdataUI(body.ToString());
                break;
            case NotiConst.UPDATE_FORCE_CHECKFAIL: //强制更新检查失败
                 ForceUpdataCheckFailUI();
                break;
            case NotiConst.UPDATE_FORCE_NONE: //强制更新检查    不需要强制更新
                 ForceUpdataCheckNoneUI();
                break;
        }
    }
GComponent loadingView;
    public void ForceUpdataCheckUI(){
        loadingView=  UIPackage.CreateObject ("public", "PublicLoadingPop").asCom;
        loadingView.SetSize (GRoot.inst.width, GRoot.inst.height);
        loadingView.AddRelation (GRoot.inst, RelationType.Size);
        loadingView.GetChild("Text").asTextField.text = "正在检查更新";
        GRoot.inst.AddChild (loadingView);
    }

     public void ForceUpdataUI(string url){
          if (loadingView!=null)
          {
              loadingView.Dispose();
          }
        view.GetController("type").selectedIndex = 2;
         view.GetChild("UpDateGameBtn").asButton.onClick.Set(()=>{
           Application.OpenURL(url);
        });
    }

      public void ForceUpdataCheckFailUI(){
          Debug.Log("ForceUpdataCheckFailUI");
          if (loadingView!=null)
          {
              loadingView.Dispose();
          }

          if (failView!=null)
          {
              return;
          }

        failView  =  UIPackage.CreateObject ("public", "PromptPop").asCom;

        failView.SetSize (GRoot.inst.width, GRoot.inst.height);
        failView.AddRelation (GRoot.inst, RelationType.Size);
        GRoot.inst.AddChild (failView);
        failView.GetController("Prompt").selectedIndex = 11;
        failView.GetChild("Text2").asTextField.text = "检查更新失败，请确认网络连接后重试";
        failView.GetChild("n9").asButton.onClick.Set(()=>{
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return;
            }
            failView.Dispose();
            failView = null;
              Debug.Log("main");
           AppFacade.Instance.StartUp();   //启动游戏
            Debug.Log("StartUp");
            if (gameManager==null)
            {
                gameManager = new GameManager();
            }
                gameManager.AddComponent();
                Timers.inst.Add(1,1,(pa)=>{
                gameManager.Init();
                });
               
                //gameManager.CheckForceUpdata();
        });
        failView.GetChild("CloseBtn").asButton.visible = false;

        
         Debug.Log("检查强制更新失败UI");

         
        
    }

     public void ForceUpdataCheckNoneUI(){
          if (loadingView!=null)
          {
              loadingView.Dispose();
          }
         view.GetController("type").selectedIndex = 0;
    }

    public void UpdatePbUI () {
        if (pb.value < 99) {
            pb.value = pb.value + 0.5;
        }
    }

    public void UpdateMessage (string data) {
       
        Debug.Log (data);
        if (data.Contains("正在解包文件"))
        {   
             this.message = data+"%";
           double value = double.Parse(data.Replace("正在解包文件",""));
            pb.value = value;
        }else if(data.Contains("正在下载更新文件")){
            double value = double.Parse(data.Replace("正在下载更新文件",""));
             this.message = data+"%";
            pb.value = value;
        }
        else{
            this.message = data;
        }
       
    }

    public void UpdateExtract (string data) {
        this.message = data;
        Debug.Log (data);

        UpdatePbUI ();
    }

    public void UpdateDownload (string data) {
        this.message = data;
        Debug.Log (data);

        UpdatePbUI ();
    }

    public void UpdateProgress (string data) {
        this.message = data;
        Debug.Log (data);
    }
    int count = 0;
    void OnGUI () {
        if (count == 0) {
            if (message.Contains ("更新失败")) {
                message = "更新失败!";
                ForceUpdataCheckFailUI();
                
            }
            if (this.updateDetail!=null && message!=null)
            {
                this.updateDetail.text = message;
            }
            
        }

        if ("更新完成!".Equals (message) && count == 0) {
            pb.value = 100;
            count++;
            view.Dispose ();
            Debug.Log ("更新完成!!");
        }
        // GUI.Label(new Rect(10, 120, 960, 50), message);

        // GUI.Label(new Rect(10, 0, 500, 50), "(1) 单击 \"Lua/Gen Lua Wrap Files\"。");
        // GUI.Label(new Rect(10, 20, 500, 50), "(2) 运行Unity游戏");
        // GUI.Label(new Rect(10, 40, 500, 50), "PS: 清除缓存，单击\"Lua/Clear LuaBinder File + Wrap Files\"。");
        // GUI.Label(new Rect(10, 60, 900, 50), "PS: 若运行到真机，请设置Const.DebugMode=false，本地调试请设置Const.DebugMode=true");
        // GUI.Label(new Rect(10, 80, 500, 50), "PS: 加Unity+ulua技术讨论群：>>341746602");
    }
}