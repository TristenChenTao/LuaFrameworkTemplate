require "Common/define"
require "View/LoginPanel"

---@class LoginCtrl
LoginCtrl = {}
local this = LoginCtrl

---@type WindowBase
local panel

--构建函数--
function LoginCtrl:New()
    logWarn("LoginCtrl.New--->>")

    panel = LoginPanel:New()

    return this
end

function LoginCtrl:Awake()
    logWarn("LoginCtrl.Awake--->>")

    -- 监听通知事件
    Event.AddListener(
        EventTypes.LoginEvent,
        function(...)
            this:HandleUIEvent(...)
        end
    )
    Event.AddListener(
        EventTypes.Download,
        function(...)
            this:HandleDownload(...)
        end
    )
    Event.AddListener(
        EventTypes.Share,
        function(...)
            this:HandleShare(...)
        end
    )
    Event.AddListener(
        EventTypes.WechatPay,
        function(...)
            this:HandleWechatPay(...)
        end
    )
    Event.AddListener(
        EventTypes.IosPurchase,
        function(...)
            this:HandleIosPurchase(...)
        end
    )

    Event.AddListener(
        EventTypes.AliPay,
        function(...)
            this:HandleAliPay(...)
        end
    )

    panel:Show()
end

function LoginCtrl:Close()
    logWarn("LoginCtrl.Close--->>")

    panel:Hide()
end

function LoginCtrl:HandleUIEvent(value1)
    logWarn("value1 is 111111111111" .. value1)

    -- 场景切换
    -- local ctrl = CtrlManager.GetCtrl(ControllerNames.Main)
    -- if ctrl ~= nil then
    --    ctrl:Awake()
    --    this:Close()
    -- end

    -- FariyGUI 控制器切换
    -- panel.contentPane:GetController("c1").selectedIndex = 1

    -- 三方登录
    ThirdPlatformTool.Authorize(ThirdPlatformType.WeChat, this.AuthorResponse)

    -- 三方分享
    -- local content = ShareContent();
    -- content:SetText("this is a test string.");
    -- content:SetImageUrl("http://ww3.sinaimg.cn/mw690/be159dedgw1evgxdt9h3fj218g0xctod.jpg");
    -- content:SetTitle("test title");
    -- content:SetUrl("https://www.jianshu.com/");
    -- content:SetShareType(ContentType.Webpage);

    -- ThirdPlatformTool.Share(ThirdPlatformType.WeChat, content, this.ShareResponse)
end

function LoginCtrl.AuthorResponse(state, message, platformType, userInfo, authInfo)
    logWarn("LoginCtrl.AuthorResponse--->>")

    logWarn("state is " .. state)
    logWarn("platformType is " .. platformType) -- ThirdPlatformType

    if state == ThirdResponseState.Sucess then
        logWarn("message is " .. message)
        logWarn("userInfo is " .. userInfo)
        logWarn("authInfo is " .. authInfo)

        local cjson = require "cjson"

        local jsonData = cjson.decode(userInfo)

        logWarn("openid is" .. jsonData["openid"])
    end
end

function LoginCtrl.ShareResponse(state, message, platformType)
    logWarn("LoginCtrl.ShareResponse--->>")

    logWarn("state is " .. state)
    logWarn("message is " .. message)
    logWarn("platformType is " .. platformType)
end



function LoginCtrl:HandleDownload(value1)
    log("download")
end



function LoginCtrl:HandleShare(value1)
    log("Share")
    local content = ShareContent()
    content:SetText("this is a test string.")
    content:SetImageUrl("http://ww3.sinaimg.cn/mw690/be159dedgw1evgxdt9h3fj218g0xctod.jpg")
    content:SetTitle("test title")
    content:SetUrl("https://www.jianshu.com/")
    content:SetShareType(ContentType.Webpage)

    ThirdPlatformTool.Share(ThirdPlatformType.WeChat, content, this.ShareResponse)
end

function LoginCtrl:HandleIosPurchase(value1)
    log("IosPurchase")
    local payManager = PayManager.GetInstence()
    local ProductID = { "YYM1", "YYM6", "YYM30", "YYM68", "YYM128", "YYM648" }
    for i,v in ipairs(ProductID) do
        PayManager.addIosPurpase(v)
    end
    local currentProduct = ProductID[indexOfList]
    print(cjson.encode(currentProduct))
    payManager:Pay(
        currentProduct,
       "",
       "",
       function()
        ShowCommonPop("支付成功")
           print("支付成功")
       end,
       function()
        ShowCommonPop("支付失败")
         print("支付失败")
       end
   )  
end

function LoginCtrl:HandleWechatPay(value1)
    log("WechatPay")
    local token = "6a9fa9c7-771d-4bfe-a6c0-fe2ad8483f20"
    local PayType =  PayType.Wechat .. ""
    local productId = "YYM1"

    PayManager.GetInstence():Pay(
        productId,
         token,
        PayType,
        function()
            print("支付成功")
        end,
        function()
            print("支付失败")
        end
    )
end


function LoginCtrl:HandleAliPay(value1)
    log("AliPay111")
    local token = "6a9fa9c7-771d-4bfe-a6c0-fe2ad8483f20"
    local PayType =  PayType.AliPay .. ""
    local productId = "YYM1"

    PayManager.GetInstence():Pay(
        productId,
         token,
        PayType,
        function()
            print("支付成功")
        end,
        function()
            print("支付失败")
        end
    )
end
