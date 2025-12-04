using RookieCartingClub.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace RookieCartingClub.Systems
{
    [UpdateInGroup(typeof(CartPhysicsSimulationGroup))]
    [UpdateBefore(typeof(PlaneCalculateSystem))]
    public partial struct EngineTorqueSystem : ISystem
    {
        private ComponentLookup<EngineData> _engineLookup;
        private ComponentLookup<PlaneResistantCollector> _planeResistantLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _engineLookup = state.GetComponentLookup<EngineData>(true);
            _planeResistantLookup = state.GetComponentLookup<PlaneResistantCollector>(true);
            state.RequireForUpdate<EngineWheelData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _engineLookup.Update(ref state);
            _planeResistantLookup.Update(ref state);
            var ecb = new EntityCommandBuffer(Allocator.TempJob);


            var job = new EngineTorqueJob
            {
                EngineLookup = _engineLookup,
                PlaneResistantLookup = _planeResistantLookup,
                CommandBuffer = ecb.AsParallelWriter()
            };

            var handle = job.ScheduleParallel(state.Dependency);
            handle.Complete();

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }

    [BurstCompile]
    public partial struct EngineTorqueJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<EngineData> EngineLookup;
        [ReadOnly] public ComponentLookup<PlaneResistantCollector> PlaneResistantLookup;

        public EntityCommandBuffer.ParallelWriter CommandBuffer;

        private void Execute([ChunkIndexInQuery] int chunkIndex, Parent parent,
            LocalToWorld transform,
            EngineWheelData wheelData,
            ref DynamicBuffer<ForceApplyRequest> forceApply)
        {
            var engine = EngineLookup[parent.Value];

            var engineBreaking = PlaneResistantLookup[wheelData.EngineResistant];
            engineBreaking.EfficiencyFactor = engine.CurrentForce == 0f ? 1 : 0;

            CommandBuffer.SetComponent(chunkIndex, wheelData.EngineResistant, engineBreaking);

            if (engineBreaking.EfficiencyFactor != 0)
                return;

            var force = transform.Forward * (wheelData.Part * engine.CurrentForce);
            forceApply.Add(new ForceApplyRequest { Force = force });
        }
    }
}