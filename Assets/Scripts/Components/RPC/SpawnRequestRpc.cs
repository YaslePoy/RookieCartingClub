using Unity.NetCode;

namespace RookieCartingClub.Components.RPC
{
    public struct SpawnRequestRpc : IRpcCommand
    {
        public CartData PlayerData;
    }
}