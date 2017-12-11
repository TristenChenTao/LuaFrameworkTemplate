require "WindowBase"

------登录界面
if (not MainPanel) then
    MainPanel = fgui.window_class()
    MainPanel.className = "MainPanel"
    setmetatable(MainPanel, WindowBase)
    ExUIPackage.AddPackage("main")
    MainPanel:SetContentSource("main", "mainPage")
end

function MainPanel:DoInit()

end

function MainPanel:DoShowAnimation()
    -- 不需要窗口弹出的动效
end
