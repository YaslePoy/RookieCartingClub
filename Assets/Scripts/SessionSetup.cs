using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

namespace DefaultNamespace
{
    public static class SessionSetup
    {
        public static ISession RequestedSession;
        public static string Nickname;
    }

    public interface ISession
    {
    }

    public class LocalSession : ISession
    {
        
    }
    
    public class NetworkSession : ISession
    {
        public string Ip;
        public ushort Port;
    }

    public class ServerSession : ISession
    {
        public ushort Port;
        public string SessionTimetable;
    }
}