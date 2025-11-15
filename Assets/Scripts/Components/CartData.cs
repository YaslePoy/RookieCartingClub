using Unity.Collections;
using Unity.Entities;

namespace RookieCartingClub.Components
{
    public struct CartData : IComponentData
    {
        public FixedString32Bytes Nickname;
        public int PlayerId;
    }
}
