using Unity.Entities;

namespace RookieCartingClub.Components
{
    public struct CurrentContactingSegment : IBufferElementData
    {
        public int Index;
    }
}