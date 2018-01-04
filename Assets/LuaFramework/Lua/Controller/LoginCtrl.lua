require "Common/define"
require "View/LoginPanel"

---@class LoginCtrl
LoginCtrl = {};
local this = LoginCtrl;

---@type WindowBase
local panel;

--构建函数--
function LoginCtrl:New()
    logWarn("LoginCtrl.New--->>")

    panel = LoginPanel:New()

    return this;
end

function LoginCtrl:Awake()
    logWarn("LoginCtrl.Awake--->>")

    -- 监听通知事件
    Event.AddListener(EventTypes.LoginEvent, function( ... )
        this:HandleUIEvent(...)
    end)

    panel:Show()
end

function LoginCtrl:Close()
    logWarn("LoginCtrl.Close--->>")

    panel:Hide();
end

function LoginCtrl:HandleUIEvent(value1)
    logWarn("value1 is 111111111111"..value1)

    -- 场景切换
    -- local ctrl = CtrlManager.GetCtrl(CtrlNames.Main)
    -- if ctrl ~= nil then
    --    ctrl:Awake()
    --    this:Close()
    -- end

    -- FariyGUI 控制器切换
    -- panel.contentPane:GetController("c1").selectedIndex = 1

    -- 三方登录
    -- ThirdPlatformTool.Authorize(ThirdPlatformType.QQ, this.AuthorResponse)

    -- 三方分享
    local content = ShareContent();
    content:SetText("this is a test string.");
    content:SetImageUrl("http://ww3.sinaimg.cn/mw690/be159dedgw1evgxdt9h3fj218g0xctod.jpg");
    content:SetTitle("test title");
    content:SetUrl("http://qjsj.youzu.com/jycs/");
    content:SetShareType(ContentType.Webpage);

    ThirdPlatformTool.Share(ThirdPlatformType.WeChat, content, this.ShareResponse)
end

function LoginCtrl.AuthorResponse(state, message, platformType, userInfo, authInfo)
    logWarn("LoginCtrl.AuthorResponse--->>")

    logWarn("state is "..state)
    logWarn("platformType is "..platformType) -- ThirdPlatformType

    if state == ThirdResponseState.Sucess then
        logWarn("message is "..message)
        logWarn("userInfo is "..userInfo)
        logWarn("authInfo is "..authInfo)

        local cjson = require "cjson"

        local jsonData = cjson.decode(userInfo)
            
        logWarn("openid is"..jsonData['openid']);
        
    end
end


function LoginCtrl.ShareResponse(state, message, platformType)
    logWarn("LoginCtrl.ShareResponse--->>")

    logWarn("state is "..state)
    logWarn("message is "..message) 
    logWarn("platformType is "..platformType)

end