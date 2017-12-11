require "FairyGUI"

GRoot.inst:SetContentScaleFactor(1080,1920)

---@class WindowBase
WindowBase = fgui.window_class()
WindowBase.className = "WindowBase"
WindowBase.animation = { "", ""}
WindowBase.asyncCreate = false

function WindowBase:ctor()
    self.vars = {}
    self._originPos = Vector2.New()

    -- 默认关闭掉点击时GRoot自动BringToFont的功能，需要再自行打开，否则在非Show弹出的窗口（parent不是GRoot）的情况下会异常
    self.bringToFontOnClick = false
end

function WindowBase:Popup()
    GRoot.inst:ShowPopup(self)
end

function WindowBase:SetContentSource(pkg, item)
    -- self:AddUISource(UISource.New(pkg))

    print(pkg)
    print(item)
    self._packageName = pkg;
    self._itemId = item
end

function WindowBase:SetDepends(...)
    local arg = {...}
    for i,v in ipairs(arg) do
        self:AddUISource(UISource(v))
    end
end

function WindowBase:Refresh()
end

function WindowBase:DefaultQCMDHandler(cmd, data)
    self:CloseModalWait()
    if data.code~=0 then
        AlertWin.Open(data.msg)
    else
        self:Refresh()
    end
end

--inheriteds----------
function WindowBase:OnInit()
    print("create Windows -- " .. self.className .. ": " .. self._packageName .. self._itemId);

    if (self._itemId ~= "") then
        if self.asyncCreate then
            UIPackage.CreateObjectAsync(self._packageName, self._itemId,
            FairyGUI.UIPackage.CreateObjectCallback(
            function(obj)
                self.contentPane = obj
                self:OnInit2()
                if self.isShowing then
                    self:DoShowAnimation()
                end
            end
            ))
        else
            self.contentPane = UIPackage.CreateObject(self._packageName, self._itemId)
            self:OnInit2()
        end
    else
        self:OnInit2()
    end

end

function WindowBase:OnInit2()
    self:SetPivot(0.5, 0.5)
    self:Center()

    -- 自动创建子控件对应的lua域引用
    AutoCreateCompChild(self.contentPane, self)


    --[[local fullScreen = self.width == 1136 and self.height == 640
    if self.frame ~=nil and fullScreen then
        self.x = math.ceil((GRoot.inst.width - self.width)/2)
        if GRoot.inst.width < self.width then
            if self.closeButton ~= nil then
                self.closeButton.x = math.floor((GRoot.inst.width + self.width) /2- self.closeButton.width)
            end
        else
            self.frame.width = GRoot.inst.width
            self.frame.x = -self.x
        end
        self.frame.height = GRoot.inst.height
    elseif fullScreen then
        self:SetSize(GRoot.inst.width, GRoot.inst.height)
    end
]]
    self:DoInit()
end

function WindowBase:DoInit()
end

function WindowBase:OnShown()

end

function WindowBase:OnHide()

end

function WindowBase:ReadyToUpdate()
end

function WindowBase:DoShowAnimation()
    if self.contentPane==nil then return end

    self:ReadyToUpdate()

    self._originPos = self.xy

    local ani = self.animation[1]
    if ani=="eject" then
        self:SetScale(0.9, 0.9)
        local tween = self:TweenScale(Vector2.New(1, 1), 0.3)
        TweenUtils.SetEase(tween, Ease.OutBack)
        TweenUtils.OnComplete(tween, WindowBase.CallOnShown, self)
    elseif ani=="fade_in" then
        self.alpha = 0
        local tween = self:TweenFade(1, 0.3)
        TweenUtils.SetEase(tween, Ease.OutQuad)
        TweenUtils.OnComplete(tween, WindowBase.CallOnShown, self)
    elseif ani=="move_up" then
        self.y = GRoot.inst.height
        local tween = self:TweenMoveY(self._originPos.y, 0.3)
        TweenUtils.SetEase(tween, Ease.OutQuad)
        TweenUtils.OnComplete(tween, WindowBase.CallOnShown, self)
    elseif ani=="move_left" then
        self.x = GRoot.inst.width
        local tween = self:TweenMoveX(self._originPos.x, 0.3)
        TweenUtils.SetEase(tween, Ease.OutQuad)
        TweenUtils.OnComplete(tween, WindowBase.CallOnShown, self)
    elseif ani=="move_right" then
        self.x = -self.width-30
        local tween = self:TweenMoveX(self._originPos.x, 0.3)
        TweenUtils.SetEase(tween, Ease.OutQuad)
        TweenUtils.OnComplete(tween, WindowBase.CallOnShown, self)
    else
        self:CallOnShown()
    end
end

function WindowBase:DoHideAnimation()
    self._originPos = self.xy
    local ani = self.animation[2]
    if ani=="shrink" then
        self:SetScale(1, 1);
        local tween = self:TweenScale(Vector2.New(0.8, 0.8), 0.2)
        TweenUtils.SetEase(tween, Ease.InExpo)
        TweenUtils.OnComplete(tween, WindowBase.DoHide, self)
    elseif ani=="move_down" then
        local tween = self:TweenMoveY(GRoot.inst.height + 30, 0.3)
        TweenUtils.SetEase(tween, Ease.OutQuad)
        TweenUtils.OnComplete(tween, WindowBase.DoHide, self)
    elseif ani=="move_left" then
        local tween = self:TweenMoveX(-self.width-30, 0.3)
        TweenUtils.SetEase(tween, Ease.OutQuad)
        TweenUtils.OnComplete(tween, WindowBase.DoHide, self)
    elseif ani=="move_right" then
        local tween = self:TweenMoveX(GRoot.inst.width + 30, 0.3)
        TweenUtils.SetEase(tween, Ease.OutQuad)
        TweenUtils.OnComplete(tween, WindowBase.DoHide, self)
    else
        self:DoHide()
    end
end

function WindowBase:CallOnShown()
    self:SetScale(1, 1)
    self.alpha = 1
    self.xy = self._originPos

    self:OnShown()
end

function WindowBase:DoHide()
    self:SetScale(1, 1)
    self.alpha = 1
    self.xy = self._originPos
    self:HideImmediately()
end

