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

        var positionsCollections = SystemAPI.GetSingletonBuffer<TrackPositionsCollection>().AsNativeArray();

        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        var job = new CartPlacementJob
        {
            PositionsLookup = _positionsLookup,
            CommandBuffer = ecb,
            PositionsCollection = positionsCollections
        };
        job.Schedule(state.Dependency).Complete();

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    [BurstCompile]
    public partial struct CartPlacementJob : IJobEntity
    {
        public EntityCommandBuffer CommandBuffer;
        public BufferLookup<TrackPlacementPosition> PositionsLookup;
        public NativeArray<TrackPositionsCollection> PositionsCollection;

        private void Execute(Entity entity,
            ref LocalTransform transform, 
            ref PhysicsVelocity velocity,
            TrackPlacementRequest request,
            EnabledRefRW<TrackPlacementRequest> enabledRequest,
            EnabledRefRW<Simulate> simulate)
        {
            var location = PositionsLookup[PositionsCollection[request.CollectionId].BufferEntity];
            var position = location[0];
            transform.Position = position.Position;
            transform.Rotation = position.Rotation;

            velocity.Linear = float3.zero;
            velocity.Angular = float3.zero;

            CommandBuffer.SetComponentEnabled<EnableSimulate>(entity, true);
            
            simulate.ValueRW = false;
            enabledRequest.ValueRW = false;
        }
    }
}


public struct EnableSimulate : IComponentData, IEnableableComponent
{
}