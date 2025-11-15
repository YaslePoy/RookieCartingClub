using System;

namespace DefaultNamespace
{
    public static class SessionSetup
    {
        public static ISession RequestedSession;
        public static string Nickname;
        public static int Id = new Random().Next(0, int.MaxValue);
    }
}