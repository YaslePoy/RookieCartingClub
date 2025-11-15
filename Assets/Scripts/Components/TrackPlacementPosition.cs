using Unity.Entities;
using Unity.Mathematics;

public struct TrackPlacementPosition : IBufferElementData
{
    public float3 Position;
    public quaternion Rotation;
}