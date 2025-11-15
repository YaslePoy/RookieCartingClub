using Unity.Entities;
using Unity.Mathematics;

public struct LocalVelocity : IComponentData
{
    public float3 Velocity;
    public float3 LastPosition;
}