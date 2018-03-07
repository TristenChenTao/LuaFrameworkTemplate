
require "Common/define"
require "Common/protocal"
require "Common/functions"

local cjson = require "cjson"

Event = require 'events'

Network = {};
local this = Network;
local decrptDESKey = nil
local islogging = false;

function Network.Start() 
    logWarn("Network.Start!!");
    Event.AddListener(Protocal.Connect, this.OnConnect); 
    Event.AddListener(Protocal.Message, this.OnMessage); 
    Event.AddListener(Protocal.Exception, this.OnException); 
    Event.AddListener(Protocal.Disconnect, this.OnDisconnect); 
    Event.AddListener(Protocal.PingTime, this.OnPing); 
    Event.AddListener(Protocal.ClientLog, this.ClientLog)
    Event.AddListener(Protocal.ShowPopMessage, this.ShowPopMessage)
    Event.AddListener(Protocal.ReverConnectCount, this.ReverConnectCount)
    Event.AddListener(Protocal.NotReachable, this.NotReachable)
    Event.AddListener(Protocal.Reachable, this.Reachable)
end

--Socket消息--
function Network.OnSocket(key, str)

    local decrptString = str
    if tostring(key) == Protocal.Message then

        if  ((type(decrptDESKey) == "string") and (#decrptDESKey > 0) )  then
            local actionIndex = string.find(str, ':')
            if actionIndex ~= nil then
                local action = string.sub(str,3,actionIndex - 1)
                local jsonString = string.sub(str,actionIndex + 1,string.len(str))
                local desdecryptString = SocketSecret.DESDecrypt(jsonString,decrptDESKey)
                decrptString = string.sub(str,0,actionIndex)..desdecryptString
            end
        else
            local actionIndex = string.find(str, ':{')
            if actionIndex ~= nil then
                local action = string.sub(str,3,actionIndex - 1)
                local jsonString = string.sub(str,actionIndex + 1,string.len(str))
                local jsonData = cjson.decode(jsonString)
                if string.lower(action) == "login" then
                    decrptDESKey = jsonData["Data"]["DESKey"]
                end
            end
        end
        if not string.find(str,"heart") then
            print("Network 服务端发送命令"..decrptString)
        end
        
    end

    Event.Brocast(tostring(key), decrptString)
end

--当连接建立时--
function Network.OnConnect()
    islogging = true
    logWarn("Game Server connected!!")

end

--当收到消息--
function Network.OnMessage(str)

    -- -- 解析服务端 Socket 协议体数据
    -- local actionIndex = string.find(str, ':{')
    -- if actionIndex ~= nil then
    --     local action = string.sub(str,3,actionIndex - 1)
    --     logWarn("action "..action)
            
    --     local jsonString = string.sub(str,actionIndex + 1,string.len(str))
    --     logWarn("jsonString "..jsonString)
            
    --     local jsonData = cjson.decode(jsonString)
            
    --     logWarn("Message is"..jsonData['Message']);
    --     logWarn("Code is"..jsonData['Code']);
    -- else
    --     logWarn("找不协议命令")
    -- end
end

--异常断线--
function Network.OnException()
    islogging = false;
   	logWarn("OnException------->>>>");
end

--连接中断，或者被踢掉--
function Network.OnDisconnect()
    islogging = false;
    decrptDESKey= nil
    logWarn("OnDisconnect------->>>>");
end

local CheckNetState = FairyGUI.Timers.inst
--检测到无网络--
function Network.NotReachable()
    ShowNotNetPop()
    CheckNetState:Add(
        2,
        0,
        function ()
        if (not (Application.internetReachability == UnityEngine.NetworkReachability.NotReachable) )then
            HideNotNetPop()
            CheckNetState = nil
        end
    end
    )

end

--检测到网络--
function Network.Reachable()
    HideNotNetPop()
end

--ping--
function Network.OnPing(str)
   	-- logWarn("OnPing------->>>> ping ="..str);
end

--ping--
function Network.ClientLog(str)
  
end


--卸载网络监听--
function Network.Unload()
    Event.RemoveListener(Protocal.Connect, this.OnConnect); 
    Event.RemoveListener(Protocal.Message, this.OnMessage); 
    Event.RemoveListener(Protocal.Exception, this.OnException); 
    Event.RemoveListener(Protocal.Disconnect, this.OnDisconnect); 
    Event.RemoveListener(Protocal.PingTime, this.OnPing); 
    Event.RemoveListener(Protocal.ClientLog, this.ClientLog)
    Event.RemoveListener(Protocal.ShowPopMessage, this.ShowPopMessage)
    Event.RemoveListener(Protocal.ReverConnectCount, this.ReverConnectCount)
    Event.RemoveListener(Protocal.NotReachable, this.NotReachable)
    Event.RemoveListener(Protocal.Reachable, this.Reachable)
    logWarn('Unload Network...');
end


---@param data string
function Network.Send(action,str)

    local message = action

    if  ((type(str) == "string") and (#str > 0) ) then
        if  ((type(decrptDESKey) == "string") and (#decrptDESKey > 0) )  then
            DESEncryptStr =   SocketSecret.DESEncrypt (str, decrptDESKey)
            message  = action..":"..DESEncryptStr
        else
            message  = action..":"..str
        end
    end

    networkMgr:SendMessage(message)
end

--弹窗显示日志信息--
function Network.ShowPopMessage( message)
    -- ShowCommonPop(message)
 end
 

--重连次数信息--
function Network.ReverConnectCount( message)
    print(message)
 end
