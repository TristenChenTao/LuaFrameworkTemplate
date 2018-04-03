using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using LuaFramework;
using UnityEngine;
using System.Runtime.InteropServices;
public enum DisType {
    Exception,
    Disconnect,
}

public class SocketClient {
    private TcpClient client = null;
    private NetworkStream outStream = null;
    private MemoryStream memStream;
    private BinaryReader reader;

    public static System.Object clock_object = new System.Object ();

    private const int MAX_READ = 8192;
    private byte[] byteBuffer = new byte[MAX_READ];
    public bool loggedIn = false;
    System.Timers.Timer sendConnectTimer; //发起下一次重连的计时器

    System.Timers.Timer checkConnectTimer; //检测socket是否还连着
    private Thread _heartThread;
    private Thread _receiveHeartThread;

    private Thread _checkReachableThread;
    private DateTime lastReceiveHeartMillisecond;

    private NetworkReachability _Reachability = Application.internetReachability;

    private int CheckReachabilityTime = 5000;

    //用来标记发起连接的时候是否还保持连着
    private bool connectedBeforeReset = false;

    private int ReverConnectCount = 0; //重连次数
    //正在连接socket
    private bool onConnection = false;

    private string SocketAddress = "";
    private int SocketPort = 0;

    private int _networkManagerType = 0;

    // Use this for initialization
    public SocketClient () {

    }

#if UNITY_IPHONE && !UNITY_EDITOR
    [DllImport ("__Internal")]
    private static extern string getIPv6 (string mHost, string mPort);
#endif

    //"192.168.1.1&&ipv4"
    public static string GetIPv6 (string mHost, string mPort) {
#if UNITY_IPHONE && !UNITY_EDITOR
        string mIPv6 = getIPv6 (mHost, mPort);
        return mIPv6;
#else
        return mHost + "&&ipv4";
#endif
    }

    void getIPType (String serverIp, String serverPorts, out String newServerIp, out AddressFamily mIPType) {
        mIPType = AddressFamily.InterNetwork;
        newServerIp = serverIp;
        try {
            string mIPv6 = GetIPv6 (serverIp, serverPorts);
            if (!string.IsNullOrEmpty (mIPv6)) {
                string[] m_StrTemp = System.Text.RegularExpressions.Regex.Split (mIPv6, "&&");
                if (m_StrTemp != null && m_StrTemp.Length >= 2) {
                    string IPType = m_StrTemp[1];
                    if (IPType == "ipv6") {
                        newServerIp = m_StrTemp[0];
                        mIPType = AddressFamily.InterNetworkV6;
                    }
                }
            }
        } catch (Exception e) {
            Debug.Log ("GetIPv6 error:" + e);
        }

    }

    /// <summary>
    /// 注册代理
    /// </summary>
    public void OnRegister () {
        memStream = new MemoryStream ();
        reader = new BinaryReader (memStream);
        _checkReachableThread = new Thread (CheckReachable);
        _checkReachableThread.Start ();
    }

    /// <summary>
    /// 移除代理
    /// </summary>
    public void OnRemove () {
        Debug.Log ("移除代理移除代理移除代理移除代理111");
        this.Close ();
        reader.Close ();
        memStream.Close ();
        _checkReachableThread.Abort ();
    }

    /// <summary>
    /// 连接服务器
    /// </summary>
    void ConnectServer (string host, int port) {

        client = null;

        Loom.QueueOnMainThread (() => {

            PingTool.StartPing (host, (pingTime) => {
                AddEvent (Protocal.PingTime, "" + pingTime);
            });

        });

        AddEvent (Protocal.ClientLog, host + ":" + port + " 开始 ConnectServer 连接服务器");
        Debug.LogWarning ("ConnectServer 连接服务器");
        try {
            String newServerIp = "";
            AddressFamily newAddressFamily = AddressFamily.InterNetwork;

            getIPType (host, "" + port, out newServerIp, out newAddressFamily);
            if (!string.IsNullOrEmpty (newServerIp)) { host = newServerIp; }

            IPAddress[] address = Dns.GetHostAddresses (host);
            if (address.Length == 0) {
                Debug.LogError ("host invalid");
                return;
            }
    
            client = new TcpClient (newAddressFamily);

            client.SendTimeout = 1000;
            client.ReceiveTimeout = 1000;
            client.NoDelay = true;
            client.BeginConnect (host, port, new AsyncCallback (OnConnect), null);
        } catch (Exception e) {
            Debug.Log ("连接服务器连接服务器连接服务器连接服务器 1");
            AddEvent (Protocal.ClientLog, "连接服务器 失败" + e);
            CloseSocketConnect ();
            Debug.LogError (e.Message);
        }
    }

    /// <summary>
    /// 连接上服务器
    /// </summary>
    void OnConnect (IAsyncResult asr) {
        client.EndConnect (asr);

        if (client.Connected) {
            ReverConnectCount = 0;
            onConnection = false;
            lastReceiveHeartMillisecond = DateTime.Now;
            CloseHeartThread ();
            CloseReceiveHeart ();
            AddEvent (Protocal.ClientLog, "发起连接成功");
            outStream = client.GetStream ();
            client.GetStream ().BeginRead (byteBuffer, 0, MAX_READ, new AsyncCallback (OnRead), null);
            // AddEvent(Protocal.Connect, new ByteBuffer());

            _heartThread = new Thread (SendHeart);
            _heartThread.Start ();
            _receiveHeartThread = new Thread (ReceiveHeart);
            _receiveHeartThread.Start ();

            AddEvent (Protocal.Connect, "");
        } else {
            RecoverConnect ();
            Debug.Log ("<><><>> 发起连接失败 连接失败");
            AddEvent (Protocal.ClientLog, "发起连接失败 连接失败");

            AddEvent (Protocal.Disconnect, "发起连接失败 连接失败");
        }
    }

    void AddEvent (int protocal, string message) {
        if (_networkManagerType == 1) {
            NetworkManager.AddEvent (protocal, message);
        } else if (_networkManagerType == 2) {
            NetworkManager2.AddEvent (protocal, message);
        }
    }

    /// <summary>
    /// 写数据
    /// </summary>
    void WriteMessage (byte[] message) {
        MemoryStream ms = null;
        using (ms = new MemoryStream ()) {
            ms.Position = 0;
            BinaryWriter writer = new BinaryWriter (ms);

            //协议体修正(协议：[消息长度4字节][消息内容])
            // ushort msglen = (ushort)message.Length;
            int msglen = message.Length;
            writer.Write (msglen);
            writer.Write (message);
            writer.Flush ();
            if (client != null && client.Connected) {
                //NetworkStream stream = client.GetStream();
                byte[] payload = ms.ToArray ();
                outStream.BeginWrite (payload, 0, payload.Length, new AsyncCallback (OnWrite), null);
            } else {
                AddEvent (Protocal.ShowPopMessage, "写数据 socket的连接断开了");
                Debug.LogWarning ("client.connected----->>false");
            }
        }
    }

    /// <summary>
    /// 读取消息
    /// </summary>
    void OnRead (IAsyncResult asr) {
        if (client.Connected == false) {
            AddEvent (Protocal.ShowPopMessage, "读取消息 socket的连接断开了");
            Debug.LogWarning ("client.connected----->>false");
            return;
        }
        int bytesRead = 0;
        try {
            lock (clock_object) { //读取字节流到缓冲区
                client.GetStream ();
                bytesRead = client.GetStream ().EndRead (asr);
            }
            if (bytesRead < 1) { //包尺寸有问题，断线处理
                Debug.Log ("//包尺寸有问题，断线处理");
                // OnDisconnected (DisType.Disconnect, "bytesRead < 1");
                // AddEvent (Protocal.ClientLog, "关掉客户端链接 //包尺寸有问题，断线处理 \"bytesRead < 1\"");
                return;
            }
            OnReceive (byteBuffer, bytesRead); //分析数据包内容，抛给逻辑层
            lock (clock_object) { //分析完，再次监听服务器发过来的新消息
                client.GetStream ();
                Array.Clear (byteBuffer, 0, byteBuffer.Length); //清空数组
                client.GetStream ().BeginRead (byteBuffer, 0, MAX_READ, new AsyncCallback (OnRead), null);
            }
        } catch (ObjectDisposedException ex) {

        } catch (Exception ex) {
            //PrintBytes();
            Debug.Log (ex);
            Debug.Log (asr);
            AddEvent (Protocal.ClientLog, "关掉客户端链接 读取消息错误：" + ex.Message);
            OnDisconnected (DisType.Exception, ex.Message);
            RecoverConnect ();
        }
    }

    /// <summary>
    /// 丢失链接
    /// </summary>
    void OnDisconnected (DisType dis, string msg) {
        int protocal = dis == DisType.Exception ?
            Protocal.Exception : Protocal.Disconnect;

        // ByteBuffer buffer = new ByteBuffer();
        // buffer.WriteShort((ushort)protocal);
        // AddEvent(protocal, buffer);
        AddEvent (protocal, "");
        Debug.LogWarning ("Connection was closed by the server:>" + msg + " Distype:>" + dis);
    }

    /// <summary>
    /// 打印字节
    /// </summary>
    /// <param name="bytes"></param>
    void PrintBytes () {
        string returnStr = string.Empty;
        for (int i = 0; i < byteBuffer.Length; i++) {
            returnStr += byteBuffer[i].ToString ("X2");
        }
        Debug.LogError (returnStr);
    }

    /// <summary>
    /// 向链接写入数据流
    /// </summary>
    void OnWrite (IAsyncResult r) {

        try {
            outStream.EndWrite (r);
        } catch (Exception ex) {
            AddEvent (Protocal.ShowPopMessage, "向链接写入数据流 失败" + ex.Message);
            Debug.LogError ("OnWrite--->>>" + ex.Message);
        }
    }

    /// <summary>
    /// 接收到消息
    /// </summary>
    void OnReceive (byte[] bytes, int length) {
        memStream.Seek (0, SeekOrigin.End);
        memStream.Write (bytes, 0, length);
        //Reset to beginning
        memStream.Seek (0, SeekOrigin.Begin);
        while (RemainingBytes () > 2) {

            //协议体修正(协议：[消息长度4字节][消息内容])
            // ushort messageLen = reader.ReadUInt16();
            int messageLen = reader.ReadInt32 ();

            if (RemainingBytes () >= messageLen) {
                MemoryStream ms = new MemoryStream ();
                BinaryWriter writer = new BinaryWriter (ms);
                writer.Write (reader.ReadBytes (messageLen));
                ms.Seek (0, SeekOrigin.Begin);
                OnReceivedMessage (ms);
            } else {
                //Back up the position two bytes

                //协议体修正(协议：[消息长度4字节][消息内容])
                memStream.Position = memStream.Position - 4;
                // memStream.Position = memStream.Position - 2;
                break;
            }
        }
        //Create a new stream with any leftover bytes
        byte[] leftover = reader.ReadBytes ((int) RemainingBytes ());
        memStream.SetLength (0); //Clear
        memStream.Write (leftover, 0, leftover.Length);
    }

    /// <summary>
    /// 剩余的字节
    /// </summary>
    private long RemainingBytes () {
        return memStream.Length - memStream.Position;
    }

    /// <summary>
    /// 接收到消息
    /// </summary>
    /// <param name="ms"></param>
    void OnReceivedMessage (MemoryStream ms) {
        BinaryReader r = new BinaryReader (ms);
        byte[] message = r.ReadBytes ((int) (ms.Length - ms.Position));
        //int msglen = message.Length;

        ByteBuffer buffer = new ByteBuffer (message);
        var messageString = System.Text.Encoding.UTF8.GetString (message);

        if (!messageString.Contains ("s_heart")) {
            // AddEvent (Protocal.ClientLog, "收到服务端消息");
        } else {
            connectedBeforeReset = true;
            // lastReceiveHeartMillisecond = DateTime.Now;
        }
        lastReceiveHeartMillisecond = DateTime.Now;
        //协议体修正(协议：[消息长度4字节][消息内容])
        //取消协议字段
        // int mainId = buffer.ReadShort();
        // AddEvent(mainId, buffer);

        AddEvent (Protocal.Message, messageString);
    }

    /// <summary>
    /// 会话发送
    /// </summary>
    void SessionSend (byte[] bytes) {
        WriteMessage (bytes);
    }

    /// <summary>
    /// 关闭链接、心跳
    /// </summary>
    public void Close () {
        Debug.Log ("关闭链接、心跳关闭链接、心跳");
        AddEvent (Protocal.ClientLog, "关闭链接、心跳关闭链接、心跳");

        loggedIn = false;

        CloseHeartThread ();
        CloseSocketConnect ();
        CloseReceiveHeart ();
        if (sendConnectTimer != null) {
            sendConnectTimer.Stop ();
            sendConnectTimer = null;
        }

        if (checkConnectTimer != null) {
            checkConnectTimer.Stop ();
            checkConnectTimer = null;
        }
    }

    /// <summary>
    /// 关闭链接
    /// </summary>
    void CloseSocketConnect () {
        if (client != null) {
            Debug.Log ("关闭Socket链接");
            if (client.Connected) client.Close ();
            client = null;
        }
    }

    /// <summary>
    /// 关闭心跳
    /// </summary>
    void CloseHeartThread () {
        if (_heartThread != null) {
            _heartThread.Abort ();
            _heartThread = null;
        }
    }

    /// <summary>
    /// 关闭心跳检测
    /// </summary>
    void CloseReceiveHeart () {
        if (_receiveHeartThread != null) {
            _receiveHeartThread.Abort ();
            _receiveHeartThread = null;
        }
    }

    /// <summary>
    /// 发送连接请求
    /// </summary>
    public void SendConnect (string SocketAddress, int SocketPort, int type) {
        this._networkManagerType = type;
        this.SocketAddress = SocketAddress;
        this.SocketPort = SocketPort;
        ConnectServer (SocketAddress, SocketPort);
        loggedIn = true;
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    public void SendMessage (String str) {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes (str.ToCharArray ());
        SessionSend (bytes);
        // buffer.Close();
    }

    ///<summary>
    ///重连
    ///<summary>
    public void RecoverConnect () {
        lock (clock_object) {
            if (loggedIn == false) {
                return;
            }

            Loom.QueueOnMainThread (() => {
                if (Application.internetReachability == UnityEngine.NetworkReachability.NotReachable) {
                    Debug.Log ("网络已断开，请检查网络");
                    onConnection = false;
                    AddEvent (Protocal.NotReachable, "网络已断开，请检查网络");
                } else {
                    AddEvent (Protocal.Reachable, "有网络");
                    if (onConnection) {
                        Debug.Log ("正在尝试重连中 不需要发起新的重连");
                        AddEvent (Protocal.ClientLog, "正在尝试重连中 不需要发起新的重连");
                        return;
                    }

                    if (client != null && client.Connected) {
                        Debug.Log ("判断还连接着，发送心跳进行测试");
                        if (checkConnectTimer != null) {
                            checkConnectTimer.Stop ();
                            checkConnectTimer = null;
                        }

                        checkConnectTimer = new System.Timers.Timer ();
                        checkConnectTimer.Interval = 1000;
                        connectedBeforeReset = false;
                        SendMessage ("Heart");
                        checkConnectTimer.Elapsed += delegate {
                            if (connectedBeforeReset == false) {
                                Debug.Log ("connectedBeforeReset==false，开始进行重连");
                                onConnection = true;
                                Close ();
                                ConnectServer (this.SocketAddress, this.SocketPort);
                                RecoverNextConnect ();
                                connectedBeforeReset = false;
                            } else {
                                Debug.Log ("connectedBeforeReset==true");
                            }
                            checkConnectTimer.Stop ();
                        };
                        checkConnectTimer.Start ();
                    } else {
                        Debug.Log ("判断已经，发送心跳进行测试");
                        onConnection = true;
                        ConnectServer (this.SocketAddress, this.SocketPort);
                        RecoverNextConnect ();
                    }
                }

            });
        }

    }

    /// <summary>
    /// 判断是否发起下一次重连
    /// </summary>  
    private void RecoverNextConnect () {
        lock (clock_object) {
            Loom.QueueOnMainThread (() => {
                if (Application.internetReachability == UnityEngine.NetworkReachability.NotReachable) {
                    Debug.Log ("网络已断开，请检查网络");
                    onConnection = false;
                    AddEvent (Protocal.NotReachable, "网络已断开，请检查网络");
                    if (sendConnectTimer != null) {
                        sendConnectTimer.Stop ();
                        sendConnectTimer = null;
                    }
                } else {
                    AddEvent (Protocal.Reachable, "有网络");
                    ReverConnectCount += 1;
                    AddEvent (Protocal.ReverConnectCount, "" + ReverConnectCount);
                    if (sendConnectTimer != null) {
                        sendConnectTimer.Stop ();
                        sendConnectTimer = null;
                    }

                    sendConnectTimer = new System.Timers.Timer ();
                    sendConnectTimer.Interval = 3000;
                    sendConnectTimer.Elapsed += delegate {
                        if (onConnection) {
                            onConnection = false;
                            RecoverConnect ();
                        }

                        sendConnectTimer.Stop ();
                    };

                    sendConnectTimer.Start ();
                }
            });
        }

    }
    /// <summary>
    ///检测服务端心跳包
    /// </summary>  
    private void ReceiveHeart () {
        while (true) {
            try {
                Thread.Sleep (AppConst.heartInterval + 2);
                if (lastReceiveHeartMillisecond != null && DateTime.Now.Subtract (lastReceiveHeartMillisecond).TotalSeconds > 7) {
                    Debug.Log ("secondTimer_Elapsed   = " + DateTime.Now.Subtract (lastReceiveHeartMillisecond).TotalSeconds);
                    string info = "didDisconnect 当前时间超过上一次收到心跳时间判断为断掉连接";
                    AddEvent (Protocal.ClientLog, info);
                    AddEvent (Protocal.ClientLog, "secondTimer_Elapsed   = " + DateTime.Now.Subtract (lastReceiveHeartMillisecond).TotalSeconds);
                    Debug.Log (info);
                    RecoverConnect ();
                    AddEvent (Protocal.Disconnect, "服务器断开连接");
                }

            } catch (ThreadAbortException e) {
                Debug.Log ("SendHeart Thread Abort Exception message =" + e);
            } catch (Exception e) {
                Debug.Log (e);
                Debug.Log ("ReceiveHeart Couldn't catch the Thread Exception");
            }
        }
    }

    /// <summary>
    ///网络状态监测
    /// </summary>  
    private void CheckReachable () {
        Thread.Sleep (CheckReachabilityTime);

        Loom.QueueOnMainThread (() => {
            if (
                Application.internetReachability != _Reachability) {
                _Reachability = Application.internetReachability;
                RecoverConnect ();
            }
        });
    }

    /// <summary>
    ///发送心跳包
    /// </summary>  
    private void SendHeart () {
        if (AppConst.heartInterval <= 0) {
            Debug.LogError ("心跳包时间间隔小雨等于0");
            return;
        }
        while (true) {
            try {
                SendMessage ("heart");
                Thread.Sleep (AppConst.heartInterval);
            } catch (ThreadAbortException e) {
                Debug.Log ("SendHeart Thread Abort Exception message =" + e);
            } catch (Exception e) {
                Debug.Log (e);
                Debug.Log ("SendHeart Couldn't catch the Thread Exception");
            }
        }
    }
}