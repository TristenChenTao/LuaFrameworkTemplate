
namespace LuaFramework {
    public class Protocal {
        ///BUILD TABLE
        public const int Connect = 101;     //连接成功
        public const int Exception = 102;     //异常
        public const int Disconnect = 103;     //断线 

        public const int Message = 104;     //消息（业务消息） 

         public const int PingTime = 105;     //ping 的数值 

         public const int ClientLog  = 106;     //客户端日志

         public const int ReverConnectCount  = 107;     //重连次数

         public const int ShowPopMessage  = 108;     //弹窗显示日志信息
         
         public const int NotReachable  = 109;     //无网络
        public const int Reachable  = 110;     //有网络
    }
}