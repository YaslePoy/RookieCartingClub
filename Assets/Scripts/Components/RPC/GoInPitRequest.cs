using Unity.NetCode;

namespace RookieCartingClub.Components.RPC
{
    public struct GoInPitRequest : IRpcCommand
    {
        public int PlayerId;
    }
}