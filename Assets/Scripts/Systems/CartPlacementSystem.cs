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
        
        new CartPlacementJob
        {
            PositionsLookup =  _positionsLookup,
            CommandBuffer = ecb.AsParallelWriter()
        }.Schedule(state.Dependency).Complete();
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public partial struct CartPlacementJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter CommandBuffer;
        [ReadOnly]
        public BufferLookup<TrackPlacementPosition>  PositionsLookup;

        private void Execute(ref DynamicBuffer<TrackPlacementRequest> requests, DynamicBuffer<TrackPositionsCollection>  positionsCollection)
        {
            foreach (var trackPlacementRequest in requests)
            {
                var location = PositionsLookup[positionsCollection[trackPlacementRequest.CollectionId].BufferEntity];
                var position = location[0];
                CommandBuffer.SetComponent(ECBCommandOrder.SetComponent, trackPlacementRequest.Player, new LocalTransform
                {
                    Position = position.Position,
                    Rotation = position.Rotation,
                    Scale = 1f
                });
                
                CommandBuffer.SetComponent<PhysicsVelocity>(ECBCommandOrder.SetComponent, trackPlacementRequest.Player, new PhysicsVelocity());
                CommandBuffer.SetComponentEnabled<Simulate>(ECBCommandOrder.SetComponentEnabled, trackPlacementRequest.Player, false);
                CommandBuffer.SetComponentEnabled<EnableSimulate>(ECBCommandOrder.SetComponentEnabled, trackPlacementRequest.Player, true);
            }
            
            requests.Clear();
        }
    }
}

public struct EnableSimulate : IComponentData, IEnableableComponent
{
}