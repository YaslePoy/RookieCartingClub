using Unity.Entities;
using Unity.Mathematics;

namespace RookieCartingClub.Components
{
    public struct LocalVelocity : IComponentData
    {
        public float3 Velocity;
        public float3 LastPosition;
    }
}