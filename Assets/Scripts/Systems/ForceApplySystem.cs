using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;
using ForceMode = Unity.Physics.Extensions.ForceMode;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial struct ForceApplySystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    public unsafe void OnUpdate(ref SystemState state)
    {
        var basic = float3.zero;
        var angular = &basic;
        var job = new ForceApplyJob
        {
            TimeStep = SystemAPI.Time.fixedDeltaTime,
            AngularVelocity = angular
        };
        job.ScheduleParallel(state.Dependency).Complete();
        
        Debug.Log(job.AngularVelocity->y);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
}

[BurstCompile]
public unsafe partial struct ForceApplyJob : IJobEntity
{
    public float TimeStep;
    public float3* AngularVelocity;
    private void Execute(ref DynamicBuffer<FinalForceRequest> requests, PhysicsMass mass, ref PhysicsVelocity velocity,
        LocalToWorld localToWorld)
    {
        if (requests.IsEmpty)
            return;

        var rawForce = float3.zero;
        var rotateForce = float3.zero;
        var center = mass.GetCenterOfMassWorldSpace(localToWorld.Position, localToWorld.Rotation);

        for (var i = 0; i < requests.Length; i++)
        {
            var request = requests[i];
            mass.GetImpulseFromForce(request.Force, ForceMode.Force, TimeStep, out var impulse, out var imp);

            rawForce += impulse;
            rotateForce += math.cross(request.Position - center, impulse);
        }

        velocity.ApplyLinearImpulse(mass, rawForce);
        velocity.ApplyAngularImpulseWorldSpace(mass, localToWorld.Position, localToWorld.Rotation, rotateForce);
        AngularVelocity->x = rotateForce.x;
        AngularVelocity->y = rotateForce.y;
        AngularVelocity->z = rotateForce.z;
        requests.Clear();
    }
}