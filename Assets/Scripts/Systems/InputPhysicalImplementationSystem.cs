using Unity.Entities;
using Unity.Jobs;
using Unity.Physics.Systems;

[UpdateBefore(typeof(CartPhysicsSimulationGroup))]
[UpdateInGroup(typeof(PhysicsInitializeGroup))]
public partial struct InputPhysicalImplementationSystem : ISystem
{
    private ComponentLookup<CartInputData> _inputLookup;

    public void OnCreate(ref SystemState state)
    {
        _inputLookup = state.GetComponentLookup<CartInputData>(true);
        state.RequireForUpdate<CartInputData>();
    }

    public void OnUpdate(ref SystemState state)
    {
        _inputLookup.Update(ref state);
        
        var breakingJob = new BreakingJob
        {
            InputLookup = _inputLookup,
        };
        var wheelJob = new WheelRotatingSystemJob
        {
            InputLookup = _inputLookup
        };
        var engineJob = new EngineCalculateJob();
        
        var breakingHandle = breakingJob.ScheduleParallel(state.Dependency);
        var wheelHandle = wheelJob.ScheduleParallel(state.Dependency);
        var engineHandle = engineJob.ScheduleParallel(state.Dependency);

        var combinedHandle = JobHandle.CombineDependencies(breakingHandle, wheelHandle, engineHandle);
        combinedHandle.Complete();
    }
}