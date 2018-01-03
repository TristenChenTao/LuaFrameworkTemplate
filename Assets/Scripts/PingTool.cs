using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PingState
{
    Nothing = 0,
    PingIng = 1,
    CanNotConnectServer = 2,
    PingOK = 3,
}

/*
Example

 */
public class PingTool : MonoBehaviour
{
    private static string s_ip = "";
    private static System.Action<int> s_callback = null;

    private static PingTool s_unityPing = null;

    private static int s_timeout = 2;

    static PingState s_CurrentPingState = PingState.Nothing;

    public static void StartPing(string ip, System.Action<int> callback)
    {
        if (string.IsNullOrEmpty(ip)) return;
        if (callback == null) return;
        if (s_unityPing != null) return;

        s_ip = ip;
        s_callback = callback;

        GameObject go = new GameObject("UnityPing");
        DontDestroyOnLoad(go);
        s_unityPing = go.AddComponent<PingTool>();
    }

    /// <summary>
    /// 超时时间（单位秒）
    /// </summary>
    public static int Timeout
    {
        set
        {
            if (value > 0)
            {
                s_timeout = value;
            }
        }
        get { return s_timeout; }
    }

    private float totalTime = 0;
    void Update()
    {
        totalTime += Time.deltaTime;
        if (s_CurrentPingState != PingState.PingIng && totalTime > 1.0)//最少1s调用一次
        {
            totalTime = 0;
            switch (Application.internetReachability)
            {
                case NetworkReachability.ReachableViaCarrierDataNetwork: // 3G/4G
                case NetworkReachability.ReachableViaLocalAreaNetwork: // WIFI
                    {
                        StopCoroutine(this.PingConnect());
                        StartCoroutine(this.PingConnect());
                    }
                    break;
                case NetworkReachability.NotReachable: // 网络不可用
                default:
                    {
                        if (s_callback != null)
                        {
                            s_callback(-1);
                            // Destroy(this.gameObject);
                        }
                    }
                    break;
            }
        }
    }

    private void OnDestroy()
    {
        s_ip = "";
        s_timeout = 20;
        s_callback = null;

        if (s_unityPing != null)
        {
            s_unityPing = null;
        }
    }

    IEnumerator PingConnect()
    {
        // Ping網站 
        Ping ping = new Ping(s_ip);
        s_CurrentPingState = PingState.PingIng;
        int addTime = 0;
        int requestCount = s_timeout * 10; // 0.1秒 请求 1 次，所以请求次数是 n秒 x 10

        // 等待请求返回
        while (!ping.isDone)
        {
            yield return new WaitForSeconds(0.1f);

            // 链接失败
            if (addTime > requestCount)
            {
                addTime = 0;

                if (s_callback != null)
                {
                    s_callback(ping.time);
                    s_CurrentPingState = PingState.CanNotConnectServer;
                    // Destroy(this.gameObject);
                }
                yield break;
            }
            addTime++;
        }

        // 链接成功
        if (ping.isDone)
        {
            if (s_callback != null)
            {
                s_callback(ping.time);
                s_CurrentPingState = PingState.PingOK;
                // Destroy(this.gameObject);
            }
            yield return null;
        }
    }
}
