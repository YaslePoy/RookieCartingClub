using RookieCartingClub.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace RookieCartingClub.Systems
{
    [UpdateInGroup(typeof(CartPhysicsSimulationGroup))]
    [UpdateBefore(typeof(RookieCartingClub.Systems.ForceSummarySystem))]
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

            var job = new PlaneCalculateJob { CommandBuffer = ecb.AsParallelWriter() };
            job.ScheduleParallel(state.Dependency).Complete();

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }

    [BurstCompile]
    public partial struct PlaneCalculateJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter CommandBuffer;

        private void Execute([ChunkIndexInQuery] int chunkIndex, PlaneResistantCollector setup, LocalVelocity velocity, LocalToWorld localToWorld)
        {
            if (setup.EfficiencyFactor == 0 || setup.MaxForce == 0)
                return;

            var speed = math.length(velocity.Velocity);
            if (speed < 0.01f)
                return;

            var resistanceFactor = math.dot(math.normalize(velocity.Velocity), math.normalize(localToWorld.Right));

            if (math.abs(resistanceFactor) < 0.0001f)
                return;

            var forceVector = localToWorld.Right;
            if (resistanceFactor > 0)
                forceVector *= -1;

            if (speed < 0.2f)
                setup.EfficiencyFactor *= speed;

            var finalForce = forceVector * math.abs(resistanceFactor * setup.EfficiencyFactor * setup.MaxForce);
            CommandBuffer.AppendToBuffer(chunkIndex, setup.Collector, new ForceApplyRequest
            {
                Force = finalForce
            });
        }
    }
}