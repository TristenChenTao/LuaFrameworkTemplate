using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using LuaFramework;
using System.Threading;

public enum DisType
{
    Exception,
    Disconnect,
}

public class SocketClient
{
    private TcpClient client = null;
    private NetworkStream outStream = null;
    private MemoryStream memStream;
    private BinaryReader reader;

    private const int MAX_READ = 8192;
    private byte[] byteBuffer = new byte[MAX_READ];
    public static bool loggedIn = false;

    private Thread _heartThread;
    private Thread _receiveHeartThread;

    private DateTime lastReceiveHeartMillisecond;
    // Use this for initialization
    public SocketClient()
    {
    }

    /// <summary>
    /// 注册代理
    /// </summary>
    public void OnRegister()
    {
        memStream = new MemoryStream();
        reader = new BinaryReader(memStream);
    }

    /// <summary>
    /// 移除代理
    /// </summary>
    public void OnRemove()
    {
        Debug.Log("移除代理");
        this.Close();
        reader.Close();
        memStream.Close();
    }

    /// <summary>
    /// 连接服务器
    /// </summary>
    void ConnectServer(string host, int port)
    {
        client = null;
        PingTool.StartPing(host,(pingTime)=>{
            NetworkManager.AddEvent(Protocal.PingTime, ""+pingTime);
        });
        Debug.Log("ConnectServer 连接服务器");
        try
        {
            IPAddress[] address = Dns.GetHostAddresses(host);
            if (address.Length == 0)
            {
                Debug.LogError("host invalid");
                return;
            }
            if (address[0].AddressFamily == AddressFamily.InterNetworkV6)
            {
                client = new TcpClient(AddressFamily.InterNetworkV6);
            }
            else
            {
                client = new TcpClient(AddressFamily.InterNetwork);
            }
            client.SendTimeout = 1000;
            client.ReceiveTimeout = 1000;
            client.NoDelay = true;
            client.BeginConnect(host, port, new AsyncCallback(OnConnect), null);
        }
        catch (Exception e)
        {
            CloseSocketConnect();
            Debug.LogError(e.Message);
        }
    }

    /// <summary>
    /// 连接上服务器
    /// </summary>
    void OnConnect(IAsyncResult asr)
    {
        client.EndConnect(asr);

        if (client.Connected)
        {
            CloseHeartThread();
            CloseReceiveHeart();
            _heartThread = new Thread(SendHeart);
            _heartThread.Start();
            _receiveHeartThread = new Thread(ReceiveHeart);
            _receiveHeartThread.Start();

            outStream = client.GetStream();
            client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
            // NetworkManager.AddEvent(Protocal.Connect, new ByteBuffer());

            NetworkManager.AddEvent(Protocal.Connect, "");
        }
        else
        {
            Debug.Log("<><><>> 发起连接失败 连接失败");
            NetworkManager.AddEvent(Protocal.Disconnect, "发起连接失败 连接失败");
        }
    }

    /// <summary>
    /// 写数据
    /// </summary>
    void WriteMessage(byte[] message)
    {
        MemoryStream ms = null;
        using (ms = new MemoryStream())
        {
            ms.Position = 0;
            BinaryWriter writer = new BinaryWriter(ms);

            //协议体修正(协议：[消息长度4字节][消息内容])
            // ushort msglen = (ushort)message.Length;
            int msglen = message.Length;
            writer.Write(msglen);
            writer.Write(message);
            writer.Flush();
            if (client != null && client.Connected)
            {
                //NetworkStream stream = client.GetStream();
                byte[] payload = ms.ToArray();
                outStream.BeginWrite(payload, 0, payload.Length, new AsyncCallback(OnWrite), null);
            }
            else
            {
                Debug.LogWarning("client.connected----->>false");
            }
        }
    }

    /// <summary>
    /// 读取消息
    /// </summary>
    void OnRead(IAsyncResult asr)
    {
        if (client.Connected == false)
        {
            Debug.LogWarning("client.connected----->>false");
            return;
        }
        int bytesRead = 0;
        try
        {
            lock (client.GetStream())
            {         //读取字节流到缓冲区
                bytesRead = client.GetStream().EndRead(asr);
            }
            if (bytesRead < 1)
            {                //包尺寸有问题，断线处理
                OnDisconnected(DisType.Disconnect, "bytesRead < 1");
                return;
            }
            OnReceive(byteBuffer, bytesRead);   //分析数据包内容，抛给逻辑层
            lock (client.GetStream())
            {         //分析完，再次监听服务器发过来的新消息
                Array.Clear(byteBuffer, 0, byteBuffer.Length);   //清空数组
                client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
            }
        }
        catch (Exception ex)
        {
            //PrintBytes();
            Debug.Log(ex.Message);
            OnDisconnected(DisType.Exception, ex.Message);
        }
    }

    /// <summary>
    /// 丢失链接
    /// </summary>
    void OnDisconnected(DisType dis, string msg)
    {
        CloseSocketConnect();   //关掉客户端链接
        int protocal = dis == DisType.Exception ?
        Protocal.Exception : Protocal.Disconnect;

        // ByteBuffer buffer = new ByteBuffer();
        // buffer.WriteShort((ushort)protocal);
        // NetworkManager.AddEvent(protocal, buffer);
        NetworkManager.AddEvent(protocal, "");
        Debug.LogWarning("Connection was closed by the server:>" + msg + " Distype:>" + dis);
    }

    /// <summary>
    /// 打印字节
    /// </summary>
    /// <param name="bytes"></param>
    void PrintBytes()
    {
        string returnStr = string.Empty;
        for (int i = 0; i < byteBuffer.Length; i++)
        {
            returnStr += byteBuffer[i].ToString("X2");
        }
        Debug.LogError(returnStr);
    }

    /// <summary>
    /// 向链接写入数据流
    /// </summary>
    void OnWrite(IAsyncResult r)
    {

        try
        {
            outStream.EndWrite(r);
        }
        catch (Exception ex)
        {
            Debug.LogError("OnWrite--->>>" + ex.Message);
        }
    }

    /// <summary>
    /// 接收到消息
    /// </summary>
    void OnReceive(byte[] bytes, int length)
    {
        memStream.Seek(0, SeekOrigin.End);
        memStream.Write(bytes, 0, length);
        //Reset to beginning
        memStream.Seek(0, SeekOrigin.Begin);
        while (RemainingBytes() > 2)
        {

            //协议体修正(协议：[消息长度4字节][消息内容])
            // ushort messageLen = reader.ReadUInt16();
            int messageLen = reader.ReadInt32();

            if (RemainingBytes() >= messageLen)
            {
                MemoryStream ms = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(ms);
                writer.Write(reader.ReadBytes(messageLen));
                ms.Seek(0, SeekOrigin.Begin);
                OnReceivedMessage(ms);
            }
            else
            {
                //Back up the position two bytes

                //协议体修正(协议：[消息长度4字节][消息内容])
                memStream.Position = memStream.Position - 4;
                // memStream.Position = memStream.Position - 2;
                break;
            }
        }
        //Create a new stream with any leftover bytes
        byte[] leftover = reader.ReadBytes((int)RemainingBytes());
        memStream.SetLength(0);     //Clear
        memStream.Write(leftover, 0, leftover.Length);
    }

    /// <summary>
    /// 剩余的字节
    /// </summary>
    private long RemainingBytes()
    {
        return memStream.Length - memStream.Position;
    }

    /// <summary>
    /// 接收到消息
    /// </summary>
    /// <param name="ms"></param>
    void OnReceivedMessage(MemoryStream ms)
    {
        BinaryReader r = new BinaryReader(ms);
        byte[] message = r.ReadBytes((int)(ms.Length - ms.Position));
        //int msglen = message.Length;

        ByteBuffer buffer = new ByteBuffer(message);
        var messageString = System.Text.Encoding.UTF8.GetString(message);

        if (!messageString.Contains("s_heart"))
        {
            Debug.Log("服务端发送命令：" + messageString);
        }
        else
        {
            lastReceiveHeartMillisecond = DateTime.Now;
        }

        //协议体修正(协议：[消息长度4字节][消息内容])
        //取消协议字段
        // int mainId = buffer.ReadShort();
        // NetworkManager.AddEvent(mainId, buffer);

        NetworkManager.AddEvent(Protocal.Message, messageString);
    }


    /// <summary>
    /// 会话发送
    /// </summary>
    void SessionSend(byte[] bytes)
    {
        WriteMessage(bytes);
    }

    /// <summary>
    /// 关闭链接、心跳
    /// </summary>
    public void Close()
    {
        CloseHeartThread();
        CloseSocketConnect();
        CloseReceiveHeart();
    }

    /// <summary>
    /// 关闭链接
    /// </summary>
    void CloseSocketConnect()
    {
        if (client != null)
        {
            if (client.Connected) client.Close();
            client = null;
        }
        loggedIn = false;
    }

    /// <summary>
    /// 关闭心跳
    /// </summary>
    void CloseHeartThread()
    {
        if (_heartThread != null)
        {
            _heartThread.Abort();
            _heartThread = null;
        }
    }

    /// <summary>
    /// 关闭心跳检测
    /// </summary>
    void CloseReceiveHeart()
    {
        if (_receiveHeartThread != null)
        {
            _receiveHeartThread.Abort();
            _receiveHeartThread = null;
        }
    }


    /// <summary>
    /// 发送连接请求
    /// </summary>
    public void SendConnect()
    {
        ConnectServer(AppConst.SocketAddress, AppConst.SocketPort);
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    public void SendMessage(String str)
    {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str.ToCharArray());
        SessionSend(bytes);
        // buffer.Close();
    }

    /// <summary>
    ///检测服务端心跳包
    /// </summary>  
    private void ReceiveHeart()
    {
        while (true)
        {
            try
            {
                Thread.Sleep(AppConst.heartInterval + 2);
                if (lastReceiveHeartMillisecond != null && DateTime.Now.Subtract(lastReceiveHeartMillisecond).TotalSeconds > 7)
                {
                    Debug.Log("secondTimer_Elapsed   = " + DateTime.Now.Subtract(lastReceiveHeartMillisecond).TotalSeconds);
                    string info = "didDisconnect 当前时间超过上一次收到心跳时间判断为断掉连接";
                    Debug.Log(info);
                    CloseHeartThread();
                    CloseSocketConnect();
                    NetworkManager.AddEvent(Protocal.Disconnect, "服务器断开连接");
                }

            }
            catch (ThreadAbortException e)
            {
                Debug.Log("SendHeart Thread Abort Exception message =" + e);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                Debug.Log("Couldn't catch the Thread Exception");
            }
        }
    }

    /// <summary>
    ///发送心跳包
    /// </summary>  
    private void SendHeart()
    {
        if (AppConst.heartInterval <= 0)
        {
            Debug.LogError("心跳包时间间隔小雨等于0");
            return;
        }
        while (true)
        {
            try
            {
                Debug.Log("客户端发送心跳");
                SendMessage("heart");
                Thread.Sleep(AppConst.heartInterval);
            }
            catch (ThreadAbortException e)
            {
                Debug.Log("SendHeart Thread Abort Exception message =" + e);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                Debug.Log("Couldn't catch the Thread Exception");
            }
        }
    }
}
