using Unity.Entities;
using Unity.Mathematics;

namespace RookieCartingClub.Components
{
    public struct ForceApplier : IComponentData
    {
        public float3 LogForce;
        public float3 LogRotation;
    }
}