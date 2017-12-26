using UnityEngine;
using LuaFramework;
using System.Collections.Generic;

using FairyGUI;

public class AppView : View {
    private string message;

    private GTextField updateDetail;

    ///<summary>
    /// 监听的消息
    ///</summary>
    List<string> MessageList {
        get {
            return new List<string>()
            { 
                NotiConst.UPDATE_MESSAGE,
                NotiConst.UPDATE_EXTRACT,
                NotiConst.UPDATE_DOWNLOAD,
                NotiConst.UPDATE_PROGRESS,
            };
        }
    }

    void Awake() {
        RemoveMessage(this, MessageList);
        RegisterMessage(this, MessageList);

        GRoot.inst.SetContentScaleFactor(1080, 1920, UIContentScaler.ScreenMatchMode.MatchWidthOrHeight);               
        UIPackage.AddPackage("UI/login");           
        GComponent view = UIPackage.CreateObject("login", "loginPage").asCom; 
 
        view.SetSize(GRoot.inst.width,GRoot.inst.height);
        view.AddRelation(GRoot.inst, RelationType.Size);
        GRoot.inst.AddChild(view);

        GButton button = view.GetChild("wechatLogin").asButton;
        button.onClick.Add(() =>{
            Util.CallMethod("Game", "OnInitOK");

            view.Dispose();
        });

        this.updateDetail = view.GetChild("updateDetail").asTextField;
    }

    /// <summary>
    /// 处理View消息
    /// </summary>
    /// <param name="message"></param>
    public override void OnMessage(IMessage message) {
        string name = message.Name;
        object body = message.Body;
        switch (name) {
            case NotiConst.UPDATE_MESSAGE:      //更新消息
                UpdateMessage(body.ToString());
            break;
            case NotiConst.UPDATE_EXTRACT:      //更新解压
                UpdateExtract(body.ToString());
            break;
            case NotiConst.UPDATE_DOWNLOAD:     //更新下载
                UpdateDownload(body.ToString());
            break;
            case NotiConst.UPDATE_PROGRESS:     //更新下载进度
                UpdateProgress(body.ToString());
            break;
        }
    }

    public void UpdateMessage(string data) {
        this.message = data;
    }

    public void UpdateExtract(string data) {
        this.message = data;
    }

    public void UpdateDownload(string data) {
        this.message = data;
    }

    public void UpdateProgress(string data) {
        this.message = data;
    }

    void OnGUI() {
        // Debug.Log("update message is "+ message);

        this.updateDetail.text = message;

        // GUI.Label(new Rect(10, 220, 960, 50), message);
        // GUI.Label(new Rect(10, 0, 500, 50), "(1) 单击 \"Lua/Gen Lua Wrap Files\"。");
        // GUI.Label(new Rect(10, 20, 500, 50), "(2) 运行Unity游戏");
        // GUI.Label(new Rect(10, 40, 500, 50), "PS: 清除缓存，单击\"Lua/Clear LuaBinder File + Wrap Files\"。");
        // GUI.Label(new Rect(10, 60, 900, 50), "PS: 若运行到真机，请设置Const.DebugMode=false，本地调试请设置Const.DebugMode=true");
        // GUI.Label(new Rect(10, 80, 500, 50), "PS: 加Unity+ulua技术讨论群：>>341746602");
    }
}
