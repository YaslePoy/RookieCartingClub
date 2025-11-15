using Unity.Entities;
using Unity.Mathematics;

namespace RookieCartingClub.Components
{
    public struct FinalForceRequest : IBufferElementData
    {
        public float3 Force;
        public float3 Position;
    }
}