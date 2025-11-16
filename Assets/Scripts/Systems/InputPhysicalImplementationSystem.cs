using RookieCartingClub.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;
using Unity.Physics.Systems;

namespace RookieCartingClub.Systems
{
    [UpdateBefore(typeof(EngineTorqueJob))]
    [UpdateInGroup(typeof(CartPhysicsSimulationGroup))]
    public partial struct InputPhysicalImplementationSystem : ISystem
    {
        private ComponentLookup<CartInputData> _inputLookup;

        public void OnCreate(ref SystemState state)
        {
            _inputLookup = state.GetComponentLookup<CartInputData>(true);
            state.RequireForUpdate<CartInputData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _inputLookup.Update(ref state);

            var breakingJob = new BreakingJob
            {
                InputLookup = _inputLookup
            };
            var breakingHandle = breakingJob.ScheduleParallel(state.Dependency);
        
            var wheelJob = new WheelRotatingSystemJob
            {
                InputLookup = _inputLookup
            };
            var wheelHandle = wheelJob.ScheduleParallel(state.Dependency);
        
            var engineJob = new EngineCalculateJob();

            var engineHandle = engineJob.ScheduleParallel(state.Dependency);

            var combinedHandle = JobHandle.CombineDependencies(breakingHandle, wheelHandle, engineHandle);
            combinedHandle.Complete();
        }
    }
}