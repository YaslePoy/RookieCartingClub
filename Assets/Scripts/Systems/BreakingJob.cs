using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[BurstCompile]
public partial struct BreakingJob : IJobEntity
{
    [ReadOnly]
    public ComponentLookup<CartInputData> InputLookup;

    private void Execute(BreakingSource source, ref PlaneResistantCollector collector)
    {
        collector.EfficiencyFactor = InputLookup[source.Source].CurrentBreaks;
    }
}