using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(EngineCalculateSystem))]
public partial struct BreakingSystem : ISystem
{
    private ComponentLookup<CartInputData> _inputLookup;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _inputLookup = state.GetComponentLookup<CartInputData>(true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _inputLookup.Update(ref state);
        new BreakingJob
        {
            InputLookup = _inputLookup,
        }.ScheduleParallel(state.Dependency).Complete();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public partial struct BreakingJob : IJobEntity
    {
        [ReadOnly]
        public ComponentLookup<CartInputData> InputLookup;

        private void Execute(BreakingSource source, ref PlaneResistantCollector collector)
        {
            collector.K = InputLookup[source.Source].CurrentBreaks;
        }
    }
}