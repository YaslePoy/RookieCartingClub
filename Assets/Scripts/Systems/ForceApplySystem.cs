using System.Numerics;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Aspects;
using Unity.Physics.Extensions;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial struct ForceApplySystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new ForceApplyJob
        {
            TimeStep = SystemAPI.Time.fixedDeltaTime
        }.ScheduleParallel(state.Dependency).Complete();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
}

[BurstCompile]
public partial struct ForceApplyJob : IJobEntity
{
    public float TimeStep;

    private void Execute(ref DynamicBuffer<FinalForceRequest> requests, PhysicsMass mass, ref PhysicsVelocity velocity,
        LocalToWorld localToWorld)
    {
        if (requests.IsEmpty)
            return;
        
        var rawForce = float3.zero;
        var rotateForce = float3.zero;
        var center = mass.GetCenterOfMassWorldSpace(localToWorld.Position, localToWorld.Rotation);
        
        for (int i = 0; i < requests.Length; i++)
        {
            var request = requests[i];
            mass.GetImpulseFromForce(request.Force, ForceMode.Force, TimeStep, out var impulse, out var imp);

            rawForce += impulse;
            rotateForce += math.cross(request.Position - center, impulse);
        }

        velocity.ApplyLinearImpulse(mass, rawForce);
        velocity.ApplyAngularImpulseWorldSpace(mass, localToWorld.Position, localToWorld.Rotation, rotateForce);
    }
}