using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(CartPhysicsSimulationGroup))]
// [UpdateBefore(typeof(WheelRotatingSystem))]
public partial struct ReplaySystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<InputRecord>();
        state.RequireForUpdate<ReplayInput>();
        state.RequireForUpdate<CartInputData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var buffer = SystemAPI.GetSingletonBuffer<InputRecord>();
        if (buffer.IsEmpty)
        {
            state.EntityManager.RemoveComponent<ReplayInput>(SystemAPI.GetSingletonEntity<InputRecord>());
            return;
        }
        var first = buffer[0];
        buffer.RemoveAt(0);
        SystemAPI.SetSingleton(first.Input);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
}