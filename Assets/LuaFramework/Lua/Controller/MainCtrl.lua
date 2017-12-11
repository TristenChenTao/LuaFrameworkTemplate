require "Common/define"
require "View/MainPanel"

---@class MainCtrl
MainCtrl = {};
local this = MainCtrl;

---@type MainPanel
local panel;

--构建函数--
function MainCtrl:New()
    logWarn("MainCtrl.New--->>")

    panel = MainPanel:New()

    return this;
end

function MainCtrl:Awake()
    logWarn("MainCtrl.Awake--->>")

    panel:Show()

end

--关闭事件--
function MainCtrl:Close()
    panel:Hide();
end