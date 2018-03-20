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
	LoginEvent = "LoginEvent",
	Download = "download",
	Share = "Share",
	WechatPay = "WechatPay",
	AliPay = "AliPay",
	IosPurchase = "IosPurchase"
}

PayManager.URL_Domain = "http://test.wolf.esgame.com" ;
PayManager.WXRecharge = "/Account/WXRecharge";
PayManager.ZfbRecharge = "/Account/ZfbRecharge";
PayManager.iosPurchaseRecharge = "/Account/VerifyReceipt";


--支付平台类型
PayType=
{
    Wechat = 1,
    AliPay = 2
}

--手机平台类型
PayTypePlatformType =
{
    ios = 1,
    android = 2
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
networkMgr2 = LuaHelper.GetNetManager2()

WWW = UnityEngine.WWW
WWWForm = UnityEngine.WWWForm
GameObject = UnityEngine.GameObject
Application = UnityEngine.Application
SystemInfo = UnityEngine.SystemInfo
PlayerPrefs = UnityEngine.PlayerPrefs;
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
    Success = 1,
    Fail = 0
}

HTTPRelativeURL = {
	CreateDesk = "/MaJiang/CreateDesk", -- 创建房间(Get)
	IsExistDesk = "/MaJiang/IsExistDesk", -- 判断房间是否存在(Get)
	GetDeskCardOneCost = "/MaJiang/GetDeskCardOneCost", -- 获取钻石换房卡比例(Get)
}