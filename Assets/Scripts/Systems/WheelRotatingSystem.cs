using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct WheelRotatingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<FrontWheel>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
}

[BurstCompile]
public partial struct WheelRotatingSystemJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter _ecb;

    public void Execute(Entity entity, FrontWheel _, Parent parent, LocalTransform transform)
    {
        
    }
}