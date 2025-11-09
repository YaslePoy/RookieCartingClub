using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;


[UpdateAfter(typeof(PhysicsSimulationGroup))]
public partial struct TrackSegmentsSpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<TrackSegmentSpawnRequest>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var bufferEntities = SystemAPI.GetSingletonEntity<TrackSegmentSpawnRequest>();

        var buffer = SystemAPI.GetBuffer<TrackSegmentSpawnRequest>(bufferEntities).ToNativeArray(Allocator.Temp);

        var i = 0;
        var entityManager = state.EntityManager;
        foreach (var entity in buffer)
        {
            // var col = entityManager.CreateEntity(typeof(LocalToWorld), typeof(LocalTransform),
            //     typeof(PhysicsCollider), typeof(PhysicsWorldIndex), typeof(CheckPointData), typeof(Simulate));
            //
            // entityManager.SetComponentData(col, new PhysicsCollider { Value = entity.Collider });
            // entityManager.AddSharedComponentManaged(col, new PhysicsWorldIndex
            // {
            //     Value = 0
            // });
            // entityManager.SetComponentData(col, new CheckPointData { Index = i++ });

            entityManager.Instantiate(entity.Collider);
        }

        entityManager.DestroyEntity(bufferEntities);
        buffer.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
}