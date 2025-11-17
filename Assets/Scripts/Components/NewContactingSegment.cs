using Unity.Entities;
using Unity.NetCode;

namespace RookieCartingClub.Components
{
    [GhostComponent]
    public struct NewContactingSegment : IBufferElementData
    {
        [GhostField]
        public int Index;
    }
}