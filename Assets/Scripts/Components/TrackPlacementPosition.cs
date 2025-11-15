using Unity.Entities;
using Unity.Mathematics;

namespace RookieCartingClub.Components
{
    public struct TrackPlacementPosition : IBufferElementData
    {
        public float3 Position;
        public quaternion Rotation;
    }
}