using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace RookieCartingClub.Components
{
    [GhostComponent]
    public struct CartData : IComponentData
    {
        [GhostField]
        public FixedString32Bytes Nickname;
        [GhostField]
        public int PlayerId;
    }
}
