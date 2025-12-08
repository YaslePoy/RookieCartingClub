using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace RookieCartingClub.Components
{
    [GhostComponent]
    public struct LocalVelocity : IComponentData
    {
        [GhostField]
        public float3 Velocity;
        public float3 LastPosition;
        public int Index;
    }
}