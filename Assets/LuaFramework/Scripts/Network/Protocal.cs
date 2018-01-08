
namespace LuaFramework {
    public class Protocal {
        ///BUILD TABLE
        public const int Connect = 101;    //连接服务器
        public const int Exception = 102;     //异常掉线
        public const int Disconnect = 103;     //正常断线 

        public const int Message = 104;     //消息（业务消息） 
        public const int PingTime = 105;     //ping 的数值 
    }
}