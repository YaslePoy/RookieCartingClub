using RookieCartingClub.Components;
using RookieCartingClub.Components.RPC;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace RookieCartingClub.Systems.Netcode.Client
{
    // [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    public partial struct CartSpawnSendSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CartSpawner>();
            var builder = new EntityQueryBuilder(Allocator.Temp).WithNone<CartOnTrack>()
                .WithNone<NetworkStreamInGame>();
            state.RequireForUpdate(state.GetEntityQuery(builder));
            state.RequireForUpdate<NetworkId>();

            state.RequireForUpdate<TrackPositionsCollection>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var userData = new ConnectRequest
            {
                PlayerData = new CartData
                {
                    PlayerId = (int)(SystemAPI.Time.ElapsedTime * 100000)
                }
            };
            var raceStateEntity = SystemAPI.GetSingletonEntity<TrackPositionsCollection>(); //rc is Race Control
            var raceState = state.EntityManager.GetComponentObject<RaceState>(raceStateEntity);
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (_, entity)
                     in SystemAPI.Query<RefRO<NetworkId>>().WithEntityAccess().WithNone<NetworkStreamInGame>())
            {
                CreateRequestEntity(commandBuffer, entity, userData, raceState);
            }

            commandBuffer.Playback(state.EntityManager);
        }

        private static void CreateRequestEntity(EntityCommandBuffer commandBuffer, Entity entity,
            ConnectRequest userData,
            RaceState raceState)
        {
            commandBuffer.AddComponent<NetworkStreamInGame>(entity);
            var req = commandBuffer.CreateEntity();
            commandBuffer.AddComponent(req, new SpawnRequestRpc { PlayerData = userData.PlayerData });
            commandBuffer.AddComponent(req, new SendRpcCommandRequest { TargetConnection = entity });

            var cartHandle = new CartHandle
            {
                PlayerId = userData.PlayerData.PlayerId,
                CheckCount = 939
            };
            cartHandle.Init();
            raceState.Racers.Add(cartHandle);
        }
    }
}