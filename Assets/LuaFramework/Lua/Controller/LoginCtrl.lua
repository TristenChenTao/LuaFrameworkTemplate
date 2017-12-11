require "Common/define"


LoginCtrl = {};
local this = LoginCtrl;

local panel;

--构建函数--
function LoginCtrl.New()
    logWarn("LoginCtrl.New--->>")

    panel = LoginPanel:New()

    return this;
end

function LoginCtrl.Awake()
    logWarn("LoginCtrl.Awake--->>")

    -- 监听通知事件
    Event.AddListener(EventTypes.LoginEvent, function( ... )
        logWarn("12312313123123123")
        this.HandleUIEvent(...)
    end);

    panel:Popup()

end

function LoginCtrl.HandleUIEvent(value1, value2)
    logWarn("value1 is "..value1)
    logWarn("value2 is "..value2)
end


--关闭事件--
function LoginCtrl.Close()

end