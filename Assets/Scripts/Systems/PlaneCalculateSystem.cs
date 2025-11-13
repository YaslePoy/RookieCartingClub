using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(ForceSummarySystem))]
public partial struct PlaneCalculateSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlaneResistantCollector>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        new PlaneCalculateJob { CommandBuffer = ecb.AsParallelWriter() }.ScheduleParallel(state.Dependency).Complete();

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
}

[BurstCompile]
public partial struct PlaneCalculateJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter CommandBuffer;

    public void Execute(Entity e, PlaneResistantCollector setup, LocalVelocity velocity, LocalToWorld localToWorld)
    {
        if (setup.K == 0 || setup.MaxForce == 0)
            return;

        var speed = math.length(velocity.Velocity);
        if (speed < 0.01f) return;
        var resistanceFactor = math.dot(math.normalize(velocity.Velocity), math.normalize(localToWorld.Right));
        if (math.abs(resistanceFactor) < 0.0001f) return;

        var forceVector = localToWorld.Right;
        if (resistanceFactor > 0) forceVector *= -1;

        if (speed < 0.2f) setup.K *= speed;

        var finalForce = forceVector * math.abs(resistanceFactor * setup.K * setup.MaxForce);
        CommandBuffer.AppendToBuffer(ECBCommandOrder.AppendToBuffer, setup.Collector, new ForceApplyRequest
        {
            Force = finalForce
        });
    }
}