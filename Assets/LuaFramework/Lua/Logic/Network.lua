
require "Common/define"
require "Common/protocal"
require "Common/functions"

local cjson = require "cjson"

Event = require 'events'

Network = {};
local this = Network;

local islogging = false;

function Network.Start() 
    logWarn("Network.Start!!");
    Event.AddListener(Protocal.Connect, this.OnConnect); 
    Event.AddListener(Protocal.Message, this.OnMessage); 
    Event.AddListener(Protocal.Exception, this.OnException); 
    Event.AddListener(Protocal.Disconnect, this.OnDisconnect); 
end

--Socket消息--
function Network.OnSocket(key, str)
    Event.Brocast(tostring(key), str)

    -- logWarn("OnSocket key : "..key)
    -- logWarn("OnSocket data : "..str)
end

--当连接建立时--
function Network.OnConnect()
    logWarn("Game Server connected!!")

end

--当收到消息--
function Network.OnMessage(str)

    -- 解析服务端 Socket 协议体数据
    local actionIndex = string.find(str, ':{')
    if actionIndex ~= nil then
        local action = string.sub(str,3,actionIndex - 1)
        logWarn("action "..action)
            
        local jsonString = string.sub(str,actionIndex + 1,string.len(str))
        logWarn("jsonString "..jsonString)
            
        -- JSON 解析
        local jsonData = cjson.decode(jsonString)
            
        logWarn("Message is"..jsonData['Message']);
        logWarn("Code is"..jsonData['Code']);
    else
        logWarn("找不协议命令")
    end
end

--异常断线--
function Network.OnException()
    islogging = false;

    NetManager:SendConnect();
   	logError("OnException------->>>>");
end

--连接中断，或者被踢掉--
function Network.OnDisconnect()
    islogging = false;

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
function Network.Send(str)
    logWarn("client send data :"..str)

    networkMgr:SendMessage(str)
end