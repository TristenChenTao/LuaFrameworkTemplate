using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingTool : MonoBehaviour
{
    private static string s_ip = "";

    private Ping _Ping = null;
    private static System.Action<int> s_callback = null;

    private static GameObject _myGameObject = null;

    public static void StartPing(string ip, System.Action<int> callback)
    {
        if (string.IsNullOrEmpty(ip)) return;
        if (callback == null) return;

        s_ip = ip;
        s_callback = callback;

        if(null == _myGameObject) {
            _myGameObject = new GameObject("UnityPing");
            _myGameObject.AddComponent<PingTool>();
        }

        DontDestroyOnLoad(_myGameObject);
    }

    public static void stopPing(){
        DestroyObject(_myGameObject);
    }

    void Start(){
        SendPing();
    }

    bool isNetWorkLose = false;
    void Update() {

        if (Application.internetReachability == NetworkReachability.NotReachable) {
            Debug.Log("Ping 无网络");
            s_callback(-1);
            isNetWorkLose = true;
        }
        else if (isNetWorkLose || (null != _Ping && _Ping.isDone) && s_callback != null) {
            s_callback(_Ping.time);
            isNetWorkLose = false;
            Invoke("SendPing", 1);//每秒Ping一次
        }

    }

    void SendPing() {
        this.DestroyPing();
        _Ping = new Ping(s_ip);
    }

    void DestroyPing() {
        if (null != _Ping){
            _Ping.DestroyPing();
            _Ping = null;
        }
    }

    private void OnDestroy() {
        Debug.Log("PingTool OnDestroy");
        s_ip = "";
        s_callback = null;
    }
}
