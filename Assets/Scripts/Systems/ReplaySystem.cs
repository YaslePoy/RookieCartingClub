using RookieCartingClub.Components;
using RookieCartingClub.Components.Replay;
using Unity.Burst;
using Unity.Entities;

namespace RookieCartingClub.Systems
{
    [UpdateInGroup(typeof(CartPhysicsSimulationGroup))]
    public partial struct ReplaySystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<RecordedInput>();
            state.RequireForUpdate<ReplayPlayback>();
            state.RequireForUpdate<CartInputData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var buffer = SystemAPI.GetSingletonBuffer<RecordedInput>();
            if (buffer.IsEmpty)
            {
                state.EntityManager.RemoveComponent<ReplayPlayback>(SystemAPI.GetSingletonEntity<RecordedInput>());
                return;
            }

            var first = buffer[0];
            buffer.RemoveAt(0);
            SystemAPI.SetSingleton(first.Input);
        }
    }
}