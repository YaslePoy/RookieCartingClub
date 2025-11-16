using RookieCartingClub.Components;
using RookieCartingClub.Components.RPC;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace RookieCartingClub.Systems.Netcode.Server
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct CartSpawnSystem : ISystem
    {
        private ComponentLookup<NetworkId> _networkIdFromEntity;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CartSpawner>();

            var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<SpawnRequestRpc>()
                .WithAll<ReceiveRpcCommandRequest>();
            state.RequireForUpdate(state.GetEntityQuery(builder));
            _networkIdFromEntity = state.GetComponentLookup<NetworkId>(true);
            state.RequireForUpdate<TrackPositionsCollection>();
        }

        public void OnUpdate(ref SystemState state)
        {
            _networkIdFromEntity.Update(ref state);
            var cartPrefab = SystemAPI.GetSingleton<CartSpawner>().CartPrefab;

            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (reqSrc, rpc, reqEntity) in SystemAPI
                         .Query<RefRO<ReceiveRpcCommandRequest>, RefRO<SpawnRequestRpc>>()
                         .WithEntityAccess())
            {
                commandBuffer.AddComponent<NetworkStreamInGame>(reqSrc.ValueRO.SourceConnection);

                var networkId = _networkIdFromEntity[reqSrc.ValueRO.SourceConnection];

                var newPlayerCart = commandBuffer.Instantiate(cartPrefab);
                commandBuffer.SetComponent(newPlayerCart, new GhostOwner { NetworkId = networkId.Value });

                commandBuffer.AppendToBuffer(reqSrc.ValueRO.SourceConnection,
                    new LinkedEntityGroup { Value = newPlayerCart });
                commandBuffer.SetComponent(newPlayerCart, rpc.ValueRO.PlayerData);
                commandBuffer.DestroyEntity(reqEntity);

                RegisterCartHandle(state, rpc);
            }

            commandBuffer.Playback(state.EntityManager);
        }

        private void RegisterCartHandle(SystemState state, RefRO<SpawnRequestRpc> rpc)
        {
            var cartHandle = new CartHandle() { PlayerId = rpc.ValueRO.PlayerData.PlayerId, CheckCount = 939 };
            cartHandle.Init();
            
            var rcEntity = SystemAPI.GetSingletonEntity<TrackPositionsCollection>(); //rc is Race Control
            var raceControl = state.EntityManager.GetComponentData<RaceControl>(rcEntity);
            raceControl.Racers.Add(cartHandle);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}