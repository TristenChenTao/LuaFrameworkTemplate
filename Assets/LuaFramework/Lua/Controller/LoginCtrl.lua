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
    end);

    panel:Show()

end

function LoginCtrl:HandleUIEvent(value1)
    logWarn("value1 is "..value1)

    local ctrl = CtrlManager.GetCtrl(CtrlNames.Main)
    if ctrl ~= nil then
        ctrl:Awake()
        this:Close()
    end

end


function LoginCtrl:Close()
    logWarn("LoginCtrl.Close--->>")

    panel:Hide();
end