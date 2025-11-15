using Unity.Entities;
using Unity.Mathematics;

public struct ForceApplyRequest : IBufferElementData
{
    public float3 Force;
}