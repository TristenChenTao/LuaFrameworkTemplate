require "WindowBase"

----登录界面
if (not LoginPanel) then
    LoginPanel = fgui.window_class()
    LoginPanel.className = "LoginPanel"
    setmetatable(LoginPanel, WindowBase)
    ExUIPackage.AddPackage("login")
    LoginPanel:SetContentSource("login", "loginPage")
end

function LoginPanel:DoInit()

    -- 绑定 UI 事件
    self:BindEvent()
end

function LoginPanel:DoShowAnimation()
    -- 不需要窗口弹出的动效
end

function LoginPanel:BindEvent()

    -- 自动搜索到子控件， 变量名为 FairyGUI 中的控件名称
    self.wechatLogin.onClick:Add(function()
        log('wechat click')

        Event.Brocast(EventTypes.LoginEvent,"event with message")

        -- FariyGUI 控制器切换
        --self.contentPane:GetController("c1").selectedIndex = 1
    end)
end