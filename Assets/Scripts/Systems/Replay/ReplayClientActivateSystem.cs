using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

namespace RookieCartingClub.Systems.Replay
{
    public partial struct ReplayClientActivateSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkId>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var connection = SystemAPI.GetSingletonEntity<NetworkId>();
            state.EntityManager.AddComponent<NetworkStreamInGame>(connection);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}