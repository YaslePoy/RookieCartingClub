using RookieCartingClub.Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

namespace RookieCartingClub.Systems.Replay
{
    public partial struct ReplayClientActivateSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            if (SessionSetup.RequestedSession is not ReplaySession)
            {
                state.Enabled = false;
                return;
            }
            state.RequireForUpdate<NetworkId>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var connection = SystemAPI.GetSingletonEntity<NetworkId>();
            state.EntityManager.AddComponent<NetworkStreamInGame>(connection);
        }
    }
}