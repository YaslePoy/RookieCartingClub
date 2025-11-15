using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(CartPhysicsSimulationGroup))]
[UpdateBefore(typeof(ForceApplySystem))]
public partial struct ForceSummarySystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CartWheel>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        new ForceSummaryJob
        {
            CommandBuffer = ecb.AsParallelWriter()
        }.Schedule(state.Dependency).Complete();

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

[BurstCompile]
public partial struct ForceSummaryJob : IJobEntity
{
    // public NativeList<DebugLine> DebugLines;
    public EntityCommandBuffer.ParallelWriter CommandBuffer;

    private void Execute([ChunkIndexInQuery] int chunkIndex, ref DynamicBuffer<ForceApplyRequest> requests,
        CartWheel wheelData,
        Parent parent,
        LocalToWorld position)
    {
        var sumForce = float3.zero;
        for (var i = 0; i < requests.Length; i++)
            sumForce += requests[i].Force;

        var forceMultiplier = wheelData.ForcePart * wheelData.Friction * wheelData.Mass;
        sumForce *= forceMultiplier;

        var length = math.length(sumForce);
        if (length == 0)
            return;

        if (length > wheelData.MaxResistance)
            sumForce *= wheelData.MaxResistance / length;

        CommandBuffer.AppendToBuffer(chunkIndex, parent.Value, new FinalForceRequest
        {
            Force = sumForce,
            Position = position.Position
        });

        requests.Clear();
    }
}