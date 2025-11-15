using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;


public partial struct CartPlacementSystem : ISystem
{
    private BufferLookup<TrackPlacementPosition> _positionsLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<TrackPlacementRequest>();
        _positionsLookup = state.GetBufferLookup<TrackPlacementPosition>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _positionsLookup.Update(ref state);

        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        var job = new CartPlacementJob
        {
            PositionsLookup = _positionsLookup,
            CommandBuffer = ecb.AsParallelWriter()
        };
        job.Schedule();

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    [BurstCompile]
    public partial struct CartPlacementJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter CommandBuffer;

        [ReadOnly]
        public BufferLookup<TrackPlacementPosition> PositionsLookup;

        private void Execute(ref DynamicBuffer<TrackPlacementRequest> requests,
            DynamicBuffer<TrackPositionsCollection> positionsCollection, EnabledRefRW<Simulate> simulate)
        {
            foreach (var trackPlacementRequest in requests)
            {
                var location = PositionsLookup[positionsCollection[trackPlacementRequest.CollectionId].BufferEntity];
                var position = location[0];
                CommandBuffer.SetComponent(ECBCommandOrder.SetComponent, trackPlacementRequest.Player,
                    new LocalTransform
                    {
                        Position = position.Position,
                        Rotation = position.Rotation,
                        Scale = 1f
                    });

                CommandBuffer.SetComponent(ECBCommandOrder.SetComponent, trackPlacementRequest.Player,
                    new PhysicsVelocity());
                simulate.ValueRW = false;

                CommandBuffer.SetComponentEnabled<EnableSimulate>(ECBCommandOrder.SetComponentEnabled,
                    trackPlacementRequest.Player, true);
            }

            requests.Clear();
        }
    }
}

public struct EnableSimulate : IComponentData, IEnableableComponent
{
}