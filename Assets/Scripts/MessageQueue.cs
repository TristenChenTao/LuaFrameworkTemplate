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
    static Queue<MessageEvent> mEvents = new Queue<MessageEvent> ();

    private static Thread _CheckCommandThread;

    private static bool  isRuning;

    public static void StartQueue (System.Action<string, string> callback) {
        MessageQueue.initData ();
        _callbackForCSharp = callback;
        _CheckCommandThread = new Thread (CheckCommand);
        _CheckCommandThread.IsBackground = true;
		 isRuning = true;
        _CheckCommandThread.Start ();
       
    }

    public static void StartQueue (LuaFunction callback) {
        MessageQueue.initData ();
        _callbackForLua = callback;
        _CheckCommandThread = new Thread (CheckCommand);
        _CheckCommandThread.IsBackground = true;
		 isRuning = true;
        _CheckCommandThread.Start ();
       
    }

    static void CheckCommand () {

		
        while (isRuning) {
            try {
                while (mEvents.Count > 0) {
                    MessageEvent topEvent = mEvents.Dequeue ();
                    string key = topEvent.key;
                    string value = topEvent.value;
                    if (topEvent.delayTime > 0) {
                        int delayTime = (int) (topEvent.delayTime * 1000);

                        if (topEvent.delayBefore) {
                            System.Threading.Thread.Sleep (delayTime);
                            Debug.Log ("MessageQueue: " + System.DateTime.Now.ToLongTimeString () + " : key is " + key + " value is " + value);
                            Loom.QueueOnMainThread (() => {
                                MessageQueue.callMethod (key, value);
                            });

                        } else {
                            Debug.Log ("MessageQueue: " + System.DateTime.Now.ToLongTimeString () + " : key is " + key + " value is " + value);
                            Loom.QueueOnMainThread (() => {
                                MessageQueue.callMethod (key, value);
                            });

                            System.Threading.Thread.Sleep (delayTime);
                        }
                    } else {
                        Debug.Log ("MessageQueue: " + System.DateTime.Now.ToLongTimeString () + " : key is " + key + " value is " + value);
                        Loom.QueueOnMainThread (() => {
                            MessageQueue.callMethod (key, value);
                        });
                    }
                }
            } catch {

            }
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
        isRuning = false;
        if (_CheckCommandThread != null) {
         _CheckCommandThread.Abort ();
		 _CheckCommandThread=null;
        }

    }

    public static void addEvent (string key, string value, double delayTime = 0, bool delayBefore = true) {

        lock (_lockObject) {
            MessageEvent newEvent = new MessageEvent ();
            newEvent.key = key;
            newEvent.value = value;
            newEvent.delayTime = delayTime;
            newEvent.delayBefore = delayBefore;

            mEvents.Enqueue (newEvent);
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