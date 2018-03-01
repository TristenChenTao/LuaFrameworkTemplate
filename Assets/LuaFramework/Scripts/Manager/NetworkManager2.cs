using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;

namespace LuaFramework {
    public class NetworkManager2 : Manager {
        private SocketClient socket;
        static readonly object m_lockObject = new object();
        static Queue<KeyValuePair<int, String>> mEvents = new Queue<KeyValuePair<int, String>>();

        SocketClient SocketClient {
            get { 
                if (socket == null)
                    socket = new SocketClient();
                return socket;                    
            }
        }

        void Awake() {
            Init();
        }

        void Init() {
            SocketClient.OnRegister();
        }

        public void OnInit() {
            CallMethod("Start");
        }

        public void Unload() {
            CallMethod("Unload");
        }

        /// <summary>
        /// ִ��Lua����
        /// </summary>
        public object[] CallMethod(string func, params object[] args) {
            return Util.CallMethod("Network2", func, args);
        }

        ///------------------------------------------------------------------------------------
        public static void AddEvent(int _event, String str) {
            lock (m_lockObject) {
                mEvents.Enqueue(new KeyValuePair<int, String>(_event, str));
            }
        }

        /// <summary>
        /// ����Command�����ﲻ����ķ���˭��
        /// </summary>
        void Update() {
            if (mEvents.Count > 0) {
                while (mEvents.Count > 0) {
                    KeyValuePair<int, String> _event = mEvents.Dequeue();
                    facade.SendMessageCommand(NotiConst.DISPATCH_MESSAGE2, _event);
                }
            }
        }

        /// <summary>
        /// ������������
        /// </summary>
        public void SendConnect(string SocketAddress,int SocketPort) {
            SocketClient.SendConnect(SocketAddress,SocketPort,2);
        }

        /// <summary>
        /// ����SOCKET��Ϣ
        /// </summary>
        public void SendMessage(String str) {
            // if(!str.Contains("Heart")) {
                Debug.Log(DateTime.Now.Second +"发送客户端命令"+str);
            // }
            SocketClient.SendMessage(str);
        }

        /// <summary>
        /// 关掉socket
        /// </summary>
        public void CloseConnect() {
            SocketClient.Close();
        }

        /// <summary>
        /// ��������
        /// </summary>
        new void OnDestroy() {
            SocketClient.OnRemove();
        }
    }
}