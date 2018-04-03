using System.Collections;
using System.Collections.Generic;
using System.Threading;
using LuaInterface;
using UnityEngine;

class MessageEvent {
    public string key;
    public string value;
    public double delayTime = 0;
    public bool delayBefore = true;
}

public class MessageQueue : MonoBehaviour {
    private static MessageQueue _messageQueue = null;
    static readonly object _lockObject = new object ();
    private static LuaFunction _callbackForLua = null; //lua 方法回调
    private static System.Action<string, string> _callbackForCSharp = null; //C# 方法回调
    private static GameObject _myGameObject = null;
    static List<MessageEvent> mEvents = new List<MessageEvent> ();

    public static void StartQueue (LuaFunction callback) {
        MessageQueue.initData ();
        _callbackForLua = callback;
    }

    double totalTime = 0;

    MessageEvent topEvent = null;
    bool hasDone = false;

    private void Update () {
        try {

            if (mEvents.Count < 0) {
                return;
            }

            if (topEvent == null) {
                topEvent = mEvents[0];
                hasDone = false;
                if (topEvent.delayBefore == false) {
                    Debug.Log ("MessageQueue: " + System.DateTime.Now.ToLongTimeString () +
                        " : key is " + topEvent.key + " value is " + topEvent.value);
                    MessageQueue.callMethod (topEvent.key, topEvent.value);
                    hasDone = true;
                    totalTime = 0;
                }
            }
            totalTime += Time.deltaTime;
            if (totalTime >= topEvent.delayTime) {
                if (hasDone == false) {
                    Debug.Log ("MessageQueue: " + System.DateTime.Now.ToLongTimeString () +
                        " : key is " + topEvent.key + " value is " + topEvent.value);
                    MessageQueue.callMethod (topEvent.key, topEvent.value);
                    hasDone = true;
                }
                
                mEvents.RemoveAt(0);
                topEvent = null;
                totalTime = 0;
                hasDone = true;
            }

        } catch {

        }
    }

    private static void initData () {
        if (null == _myGameObject) {
            _myGameObject = new GameObject ("MessageQueue");
            _messageQueue = _myGameObject.AddComponent<MessageQueue> ();
            DontDestroyOnLoad (_myGameObject);
        }

        _callbackForLua = null;
        _callbackForCSharp = null;

        MessageQueue.ClearQueue ();
    }

    public static void ClearQueue () {
        mEvents.Clear ();
    }

    public static void ClearEvents () {
        mEvents.Clear ();
    }

    public static void addEvent (string key, string value, double delayTime = 0, bool delayBefore = true) {

        lock (_lockObject) {
            MessageEvent newEvent = new MessageEvent ();
            newEvent.key = key;
            newEvent.value = value;
            newEvent.delayTime = delayTime;
            newEvent.delayBefore = delayBefore;

            mEvents.Add (newEvent);
        }
    }
    public static void callMethod (string key, string value) {
        if (null != _callbackForLua) {
            _callbackForLua.Call (key, value);
        } else if (null != _callbackForCSharp) {
            _callbackForCSharp (key, value);
        }
    }
}