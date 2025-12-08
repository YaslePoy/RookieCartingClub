using Unity.Entities;
using Unity.NetCode;

namespace RookieCartingClub.Determ.Systems
{
    public struct CubeRpc : IRpcCommand
    {
        public CubeEventType Type;
    }

    public enum CubeEventType : byte
    {
        SpawnCube,
        RemoveCube,
    }
}