--主入口函数。从这里开始lua逻辑
require "Common/define"

function Main()                    
    print("logic start")   
    --begin   luaide
    -- if(UnityEngine.Application.platform == UnityEngine.RuntimePlatform.IPhonePlayer) then 
        -- local breakSocketHandle,debugXpCall = require("LuaDebugjit")("localhost",7003)
        -- local timer = Timer.New(function() 
        -- breakSocketHandle() end, 1, -1, false)
        -- timer:Start();
    -- else 
        -- local breakSocketHandle,debugXpCall = require("LuaDebug")("localhost",7003)
        -- local timer = Timer.New(function() 
        -- breakSocketHandle() end, 1, -1, false)
        -- timer:Start();
    -- end
--end luaide 


end
--场景切换通知
function OnLevelWasLoaded(level)
	collectgarbage("collect")
	Time.timeSinceLevelLoad = 0
end

function OnApplicationQuit()
    print("OnApplicationQuit1111111111");
end