using UnityEngine;
using System.Collections;
using LuaFramework;

public class StartUpCommand : ControllerCommand {

    public override void Execute(IMessage message) {
        if (!Util.CheckEnvironment()) return;

        GameObject go = GameObject.FindWithTag("GuiCamera");
        AppView appView = go.AddComponent<AppView>();

        // GameObject gameMgr = GameObject.Find("GlobalGenerator");
        // if (gameMgr != null) {
        //     AppView appView = gameMgr.AddComponent<AppView>();
        // }
        //-----------------关联命令-----------------------
        AppFacade.Instance.RegisterCommand(NotiConst.DISPATCH_MESSAGE, typeof(SocketCommand));
        AppFacade.Instance.RegisterCommand(NotiConst.DISPATCH_MESSAGE2, typeof(SocketCommand2));

        //-----------------初始化管理器-----------------------
        AppFacade.Instance.AddManager<LuaManager>(ManagerName.Lua);
        AppFacade.Instance.AddManager<PanelManager>(ManagerName.Panel);
        AppFacade.Instance.AddManager<SoundManager>(ManagerName.Sound);
        AppFacade.Instance.AddManager<TimerManager>(ManagerName.Timer);
        AppFacade.Instance.AddManager<NetworkManager>(ManagerName.Network);
        AppFacade.Instance.AddManager<NetworkManager2>(ManagerName.Network2);
        AppFacade.Instance.AddManager<ResourceManager>(ManagerName.Resource);
        AppFacade.Instance.AddManager<ThreadManager>(ManagerName.Thread);
        AppFacade.Instance.AddManager<ObjectPoolManager>(ManagerName.ObjectPool);

        AppFacade.Instance.AddManager<GameManager>(ManagerName.Game);
       
    }
}