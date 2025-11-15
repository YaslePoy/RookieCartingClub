using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

public partial struct EnableCartSimulationSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnableSimulate>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        var job = new EnableCartSimulationJob { CommandBuffer = ecb.AsParallelWriter() };
        job.ScheduleParallel(state.Dependency).Complete();

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    [BurstCompile]
    [WithAll(typeof(EnableSimulate))]
    public partial struct EnableCartSimulationJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter CommandBuffer;

        private void Execute(Entity cart)
        {
            CommandBuffer.SetComponentEnabled<Simulate>(ECBCommandOrder.SetComponentEnabled, cart, true);
            CommandBuffer.SetComponentEnabled<EnableSimulate>(ECBCommandOrder.SetComponentEnabled, cart, false);
        }
    }
}