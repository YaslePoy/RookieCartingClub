using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace RookieCartingClub.Components
{
    [GhostComponent]
    public struct CartData : IComponentData
    {
        public FixedString32Bytes Nickname;
        public int PlayerId;
    }
}
