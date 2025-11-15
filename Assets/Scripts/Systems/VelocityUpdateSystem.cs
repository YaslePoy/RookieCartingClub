using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(CartPhysicsSimulationGroup))]
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
        var job = new VelocityUpdateJob { TimeStep = SystemAPI.Time.fixedDeltaTime };
        job.ScheduleParallel(state.Dependency).Complete();
    }
}


[BurstCompile]
public partial struct VelocityUpdateJob : IJobEntity
{
    public float TimeStep;

    private void Execute(LocalToWorld localToWorld, ref LocalVelocity velocity)
    {
        const float maximumSpeedSq = 120.0f * 120.0f; // 120 meters per second
        var currentPosition = localToWorld.Position;
        velocity.Velocity = (currentPosition - velocity.LastPosition) / TimeStep;
        velocity.LastPosition = currentPosition;
        if (math.lengthsq(velocity.Velocity) > maximumSpeedSq) velocity.Velocity = float3.zero;
    }
}