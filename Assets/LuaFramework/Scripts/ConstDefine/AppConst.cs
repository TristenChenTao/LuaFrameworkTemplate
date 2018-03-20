using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LuaFramework {
    public class AppConst {

        //正式站
        // #if UNITY_ANDROID
		//    public static string WebUrl  ="http://bigfile.esgame.com/GameUpdate/QiPai/Android/StreamingAssets/" ;  //正式更新地址
		// 	#elif UNITY_IPHONE
		//    public static string WebUrl  ="http://bigfile.esgame.com/GameUpdate/QiPai/IOS/StreamingAssets/" ;  //正式更新地址
        //     #elif UNITY_EDITOR
        //     public static string WebUrl  ="http://bigfile.esgame.com/GameUpdate/QiPai/StreamingAssets/" ;  //测试更新地址
		// 	#endif
        //  public static string  URL_Domain = "http://majiang.esgame.com";


        //测试站
              #if UNITY_ANDROID
		  public static string WebUrl  ="http://bigfile.esgame.com/GameUpdate/QiPai/DevAndroid/StreamingAssets/" ;  //测试更新地址
			#elif UNITY_IPHONE
		   public static string WebUrl  ="http://bigfile.esgame.com/GameUpdate/QiPai/DevIOS/StreamingAssets/" ;  //测试更新地址
            #elif UNITY_EDITOR
            public static string WebUrl  ="http://bigfile.esgame.com/GameUpdate/QiPai/StreamingAssets/" ;  //测试更新地址
			#endif
          public static string  URL_Domain = "http://test.majiang.esgame.com"; 

        
        public const bool DebugMode = true;                             //调试模式-用于内部测试
        public const bool LuaBundleMode = true;                       //Lua代码AssetBundle模式
        public const bool UpdateMode = false;                           //热更新模式
        public const bool ForceUpdateMode = false;                     //强制更新模式   
        public static string ForceUpdataUrl = URL_Domain+"/Version/VersionUpdate"; //强制更新url地址


        
        // public const bool DebugMode = false;                         //调试模式-用于内部测试 <打包>
        // public const bool LuaBundleMode = true;                     //Lua代码AssetBundle模式 <打包>
        // public const bool UpdateMode = true;                         //热更新模式   <打包>
        // public const bool ForceUpdateMode = true;                    //强制更新模式   <打包>
        // public static string ForceUpdataUrl = URL_Domain+"/Version/VersionUpdate"; //强制更新url地址  <打包>

        public const int designResolutionX = 1080;
        public const int designResolutionY = 1920;

        /// <summary>
        /// 如果想删掉框架自带的例子，那这个例子模式必须要
        /// 关闭，否则会出现一些错误。
        /// </summary>
        public const bool ExampleMode = false; //例子模式 

        /// <summary>
        /// 如果开启更新模式，前提必须启动框架自带服务器端。
        /// 否则就需要自己将StreamingAssets里面的所有内容
        /// 复制到自己的Webserver上面，并修改下面的WebUrl。
        /// </summary>
       
        public const bool LuaByteMode = false;                       //Lua字节码模式-默认关闭 
       
         public const string LuaDESKey = "akKp;maskBhjvalk891HJvdahjklljasjds901";  
         
        public const int TimerInterval = 1;
        public const int GameFrameRate = 30; //游戏帧频

        public const string AppName = "LuaFramework"; //应用程序名称
        public const string LuaTempDir = "Lua/"; //临时目录

        public const string AudioDir = "Audio/";                        //音频资源路径
        public const string UIDir = "UI/";                          //FairyUI 资源路径
        public const string AppPrefix = AppName + "_";              //应用程序前缀
        public const string ExtName = ".unity3d";                   //素材扩展名
        public const string AssetDir = "StreamingAssets";           //素材目录 


        

        public static string UserId = string.Empty; //用户ID
        public static int SocketPort = 0; //Socket服务器端口
        public static string SocketAddress = string.Empty; //Socket服务器地址

        public static int heartInterval = 5000; //心跳包间隔 单位毫秒

        public static string Product_Version = "1.0"; //与服务端交互的内部版本号
        public static string App_Version = "1.0"; //App展示的版本号
       

        public static string FrameworkRoot {
            get {
                return Application.dataPath + "/" + AppName;
            }
        }

    }
}