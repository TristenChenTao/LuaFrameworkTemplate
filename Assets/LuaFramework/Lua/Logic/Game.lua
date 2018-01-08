local breakSocketHandle,debugXpCall = require("LuaDebug")("localhost",7003)
local timer = Timer.New(function() 
breakSocketHandle() end, 1, -1, false)
timer:Start();

require "Logic/LuaClass"
require "Logic/CtrlManager"
require "Common/functions"

--管理器--
Game = {};
local this = Game;

--初始化完成，发送链接服务器信息--
function Game.OnInitOK()


    -- Socket 
    AppConst.SocketAddress = "119.23.173.121";
    AppConst.SocketPort = 4020;
    networkMgr:SendConnect() -- Network.lua 里处理收发消息
    Network.Send("loginauto") -- Socket 发包

    CtrlManager.Init();
    local ctrl = CtrlManager.GetCtrl(ControllerNames.Login);
    if ctrl ~= nil then
       ctrl:Awake();
    end

    logWarn('LuaFramework InitOK--->>>')

    -- Update 监听 
    UpdateBeat:Add(Update, self)

    -- HTTP  
    this.TestHTTP()

    --音乐播放
    soundMgr:LoadAudioClip("AllUse/audioStuff")
    soundMgr:PlayBacksound("AllUse/audioStuff",true) 
end

function Update()
    LuaFramework.Util.Log("每帧执行一次");
end

--销毁--
function Game.OnDestroy()
	--logWarn('OnDestroy--->>>');
end


function Game.TestHTTP()

    local parameter = WWWForm()
    parameter:AddField("test1", 1) -- 传入参数

    local url = URL_Domain..HTTPRelativeURL.GetDeskCardOneCost
	HTTPClient.Request(HTTPRequestType.Get, url,  parameter, this.ResponseHTTP)
end

function Game.ResponseHTTP(state,code,message, data)
    logWarn("Game.ResponseHTTP--->>")

    if state == HTTPResponseState.Fail then
        logWarn("请求失败")
    end

    logWarn("state is "..state)
    logWarn("code is "..state)
    logWarn("message is "..message)
    logWarn("data is "..data)
end