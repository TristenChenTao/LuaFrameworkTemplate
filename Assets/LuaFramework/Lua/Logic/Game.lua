
require "Logic/LuaClass"
require "Logic/CtrlManager"
require "Common/functions"

--管理器--
Game = {};
local this = Game;


--初始化完成，发送链接服务器信息--
function Game.OnInitOK()
    --AppConst.SocketPort = 2012;
    --AppConst.SocketAddress = "127.0.0.1";
    --networkMgr:SendConnect();

    CtrlManager.Init();
    local ctrl = CtrlManager.GetCtrl(CtrlNames.Login);
    if ctrl ~= nil then
       ctrl:Awake();
    end

    logWarn('LuaFramework InitOK--->>>');
end


--销毁--
function Game.OnDestroy()
	--logWarn('OnDestroy--->>>');
end
