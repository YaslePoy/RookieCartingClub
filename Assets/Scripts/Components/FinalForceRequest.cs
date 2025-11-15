using Unity.Entities;
using Unity.Mathematics;

public struct FinalForceRequest : IBufferElementData
{
    public float3 Force;
    public float3 Position;
}