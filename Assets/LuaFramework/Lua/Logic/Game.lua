
require "Logic/LuaClass"
require "Logic/CtrlManager"
require "Common/functions"

--管理器--
Game = {};
local this = Game;


--初始化完成，发送链接服务器信息--
function Game.OnInitOK()

    AppConst.SocketAddress = "120.24.247.165";
    AppConst.SocketPort = 3000;
    networkMgr:SendConnect() -- Network.lua 里处理收发消息

    ---- Socket 发包
    --Network.Send("loginauto")

    CtrlManager.Init();
    local ctrl = CtrlManager.GetCtrl(CtrlNames.Login);
    if ctrl ~= nil then
       ctrl:Awake();
    end

    logWarn('LuaFramework InitOK--->>>')

    --UpdateBeat:Add(Update, self)
end

function Update()
    LuaFramework.Util.Log("每帧执行一次");
end

--销毁--
function Game.OnDestroy()
	--logWarn('OnDestroy--->>>');
end
