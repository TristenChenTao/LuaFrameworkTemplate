CtrlNames = {
	Prompt = "PromptCtrl",
	Message = "MessageCtrl"
}

PanelNames = {
	"PromptPanel",	
	"MessagePanel",
}

ControllerNames = {
	Login = "LoginCtrl",
	Main = "MainCtrl"
}

EventTypes = {
	LoginEvent = "LoginEvent"
}

--协议类型--
ProtocalType = {
	BINARY = 0,
	PB_LUA = 1,
	PBC = 2,
	SPROTO = 3,
}
--当前使用的协议类型--
TestProtoType = ProtocalType.BINARY;

Util = LuaFramework.Util;
AppConst = LuaFramework.AppConst;
LuaHelper = LuaFramework.LuaHelper;
ByteBuffer = LuaFramework.ByteBuffer;

resMgr = LuaHelper.GetResManager();
panelMgr = LuaHelper.GetPanelManager();
soundMgr = LuaHelper.GetSoundManager();
networkMgr = LuaHelper.GetNetManager();

WWW = UnityEngine.WWW;
GameObject = UnityEngine.GameObject;

WWWForm = UnityEngine.WWWForm;
ShareContent = cn.sharesdk.unity3d.ShareContent;
ContentType = cn.sharesdk.unity3d.ContentType;

-- 第三方平台
ThirdPlatformType = {
	WeChat = 1,
	QQ = 2,
	Weibo = 3,
	WeChatMoments = 4
}

ThirdResponseState = {
	Sucess = 1,
	Fail = 0,
	Cancel = -1
}

-- HTTP

URL_Domain = "http://test.majiang.esgame.com";
--URL_Domain = "http://120.24.247.165:8016";

HTTPRequestType = {
	Get = 1,
	Post = 2
}

HTTPResponseState = {
	Sucess = 1,
	Fail = 0
}

HTTPRelativeURL = {
	CreateDesk = "/MaJiang/CreateDesk", -- 创建房间(Get)
	IsExistDesk = "/MaJiang/IsExistDesk", -- 判断房间是否存在(Get)
	GetDeskCardOneCost = "/MaJiang/GetDeskCardOneCost", -- 获取钻石换房卡比例(Get)
}