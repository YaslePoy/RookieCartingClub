using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
internal partial struct WheelRotatingSystem : ISystem
{
    private ComponentLookup<CartInputData> _inputDataLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _inputDataLookup = state.GetComponentLookup<CartInputData>(true);
        state.RequireForUpdate<FrontWheel>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _inputDataLookup.Update(ref state);

        var job = new WheelRotatingSystemJob
        {
            InputLookup = _inputDataLookup
        };

        job.Schedule();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
}

[BurstCompile]
public partial struct WheelRotatingSystemJob : IJobEntity
{
    [ReadOnly] public ComponentLookup<CartInputData> InputLookup;

    private void Execute(FrontWheel _, Parent parent, ref LocalTransform transform)
    {
        var input = InputLookup[parent.Value];
        var angle = input.CurrentAngle * math.TORADIANS;
        transform.Rotation = quaternion.AxisAngle(transform.Up(), angle);
    }
}