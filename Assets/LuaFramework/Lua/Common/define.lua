
CtrlNames = {
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

Util = LuaFramework.Util;
AppConst = LuaFramework.AppConst;
LuaHelper = LuaFramework.LuaHelper;
ByteBuffer = LuaFramework.ByteBuffer;

resMgr = LuaHelper.GetResManager();
panelMgr = LuaHelper.GetPanelManager();
soundMgr = LuaHelper.GetSoundManager();
networkMgr = LuaHelper.GetNetManager();

WWW = UnityEngine.WWW;
WWWForm = UnityEngine.WWWForm;
GameObject = UnityEngine.GameObject;


-- 第三方平台
AuthorizePlatformType = {
	WeChat = 1,
	QQ = 2,
	Weibo = 3
}

AuthorizeResponseState = {
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