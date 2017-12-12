
require "Common/define"
require "Common/protocal"
require "Common/functions"

Event = require 'events'

Network = {};
local this = Network;

---@type Timer
local HeartBeatTimer;

local islogging = false;

function Network.Start() 
    logWarn("Network.Start!!");
    Event.AddListener(Protocal.Connect, this.OnConnect); 
    Event.AddListener(Protocal.Message, this.OnMessage); 
    Event.AddListener(Protocal.Exception, this.OnException); 
    Event.AddListener(Protocal.Disconnect, this.OnDisconnect); 
end

--Socket消息--
function Network.OnSocket(key, data)

    Event.Brocast(tostring(key), data)

    local str = data:ReadString();
    logWarn("OnSocket data : "..str)

    logWarn("OnSocket key : "..tostring(key))
end

--当连接建立时--
function Network.OnConnect()
    logWarn("Game Server connected!!")

    Network.StartHeartBeat()
end

--当收到消息--
function Network.OnMessage(buffer)

    local str = buffer:ReadString();
    logWarn('OnMessage-------->>>'..str);
end

--异常断线--
function Network.OnException()
    islogging = false;
    HeartBeatTimer:Stop()

    NetManager:SendConnect();
   	logError("OnException------->>>>");
end

--连接中断，或者被踢掉--
function Network.OnDisconnect()
    islogging = false;

    HeartBeatTimer:Stop()

    logError("OnDisconnect------->>>>");
end


--卸载网络监听--
function Network.Unload()
    Event.RemoveListener(Protocal.Connect);
    Event.RemoveListener(Protocal.Message);
    Event.RemoveListener(Protocal.Exception);
    Event.RemoveListener(Protocal.Disconnect);
    logWarn('Unload Network...');
end


---@param data string
function Network.Send(data)
    logWarn("client send data :"..data)

    local buffer = ByteBuffer.New()

    buffer:WriteShort(Protocal.Message)
    buffer:WriteString(data)

    networkMgr:SendMessage(buffer)
end

function Network.StartHeartBeat()
    if (not HeartBeatTimer) then
        HeartBeatTimer = Timer.New(function( ... )

            Network.Send("Heart")

        end, 1, -1, true)
    end

    HeartBeatTimer:Start()
end