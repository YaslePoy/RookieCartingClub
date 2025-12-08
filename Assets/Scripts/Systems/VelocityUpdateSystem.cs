using RookieCartingClub.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace RookieCartingClub.Systems
{
    [UpdateInGroup(typeof(CartPhysicsSimulationGroup))]
    [UpdateBefore(typeof(EngineTorqueSystem))]
    [UpdateAfter(typeof(InputPhysicalImplementationSystem))]
    public partial struct VelocityUpdateSystem : ISystem
    {
        private EntityQuery _teleportedQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LocalVelocity>();

            _teleportedQuery = SystemAPI.QueryBuilder().WithAll<WasTeleported>().WithNone<Prefab>().Build();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var teleported = _teleportedQuery.ToEntityArray(Allocator.Temp);
            var teleportedSet = new NativeHashSet<Entity>(teleported.Length, Allocator.TempJob);

            foreach (var entity in teleported)
                teleportedSet.Add(entity);

            var job = new VelocityUpdateJob
            {
                TimeStep = SystemAPI.Time.fixedDeltaTime,
                Teleported = teleportedSet
            };
            var updateHandle = job.ScheduleParallel(state.Dependency);

            var refreshJob = new RemoveTeleportationJob();
            refreshJob.ScheduleParallel(updateHandle).Complete();
        }
    }

    [BurstCompile]
    public partial struct VelocityUpdateJob : IJobEntity
    {
        public float TimeStep;
        [ReadOnly] public NativeHashSet<Entity> Teleported;

        private void Execute(LocalToWorld localToWorld, ref LocalVelocity velocity)
        {
            const float maximumSpeedSq = 120.0f * 120.0f; // 120 meters per second is TOO fast
            var currentPosition = localToWorld.Position;

            switch (velocity.Initialized)
            {
                case false:
                {
                    if (currentPosition.x != 0 && currentPosition.y != 0 && currentPosition.z != 0)
                    {
                        velocity.LastPosition = currentPosition;
                        velocity.Initialized = true;
                    }
                    return;
                }
                case true:
                {
                    var wasThisTeleported = Teleported.Contains(velocity.Root);
                    if (wasThisTeleported)
                        velocity.Velocity = float3.zero;
                    else
                        velocity.Velocity = (currentPosition - velocity.LastPosition) / TimeStep;

                    velocity.LastPosition = currentPosition;
                    if (math.lengthsq(velocity.Velocity) > maximumSpeedSq) velocity.Velocity = float3.zero;
                    break;
                }
            }
        }
    }

    [BurstCompile]
    [WithAll(typeof(WasTeleported))]
    public partial struct RemoveTeleportationJob : IJobEntity
    {
        private void Execute(EnabledRefRW<WasTeleported> teleportedTag)
        {
            teleportedTag.ValueRW = false;
        }
    }
}