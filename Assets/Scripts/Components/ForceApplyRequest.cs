using Unity.Entities;
using Unity.Mathematics;

namespace RookieCartingClub.Components
{
    public struct ForceApplyRequest : IBufferElementData
    {
        public float3 Force;
    }
}