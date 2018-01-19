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
    self.wechatLogin.onClick:Add(
        function()

            Event.Brocast(EventTypes.LoginEvent, "event with message")
        end
    )
    self.ailPay.onClick:Add(
        function()
            

            Event.Brocast(EventTypes.AliPay, "event with message")
        end
    )
    self.weChatPay.onClick:Add(
        function()

            Event.Brocast(EventTypes.WechatPay, "event with message")
        end
    )
    self.download.onClick:Add(
        function()
            log("wechat click")

            Event.Brocast(EventTypes.Download, "event with message")

            --下载图片
            local imageURL = "http://wzshipin.com/d/file/201712/c8f73442b81f8024ae440d5c265af4e5.jpg"
            HTTPClient.LoadWebImage(
                imageURL,
                function(state, message, image)
                    logWarn("LoadWebImage state is " .. state)
                    logWarn("LoadWebImage message is " .. message)
                    self.image.texture = NTexture(image)
                end
            )
        end
    )
    self.share.onClick:Add(
        function()

            Event.Brocast(EventTypes.Share, "event with message")
        end
    )
end
