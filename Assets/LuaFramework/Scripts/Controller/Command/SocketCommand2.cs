using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuaFramework;

public class SocketCommand2 : ControllerCommand {

    public override void Execute(IMessage message) {
        object data = message.Body;
        if (data == null) return;
        KeyValuePair<int, string> buffer = (KeyValuePair<int, string>)data;
        switch (buffer.Key) {
            default: Util.CallMethod("Network2", "OnSocket", buffer.Key, buffer.Value); break;
        }
	}
}
