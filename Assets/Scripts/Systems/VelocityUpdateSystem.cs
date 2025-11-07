using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(PlaneCalculateSystem))]
public partial struct VelocityUpdateSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<LocalVelocity>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new VelocityUpdateJob { TimeStep = SystemAPI.Time.fixedDeltaTime }.ScheduleParallel(state.Dependency).Complete();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
}


[BurstCompile]
public partial struct VelocityUpdateJob : IJobEntity
{
    public float TimeStep;

    private void Execute(LocalToWorld localToWorld, ref LocalVelocity velocity)
    {
        var currentPosition = localToWorld.Position;
        velocity.Velocity = (currentPosition - velocity.LastPosition) / TimeStep;
        velocity.LastPosition = currentPosition;
        if (math.lengthsq(velocity.Velocity) > 14400)
        {
            velocity.Velocity = float3.zero;
        }
    }
}