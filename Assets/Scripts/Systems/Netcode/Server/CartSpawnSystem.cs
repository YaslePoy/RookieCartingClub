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
        }

        [BurstCompile]
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
                commandBuffer.SetComponent(newPlayerCart, new CartData { PlayerId = rpc.ValueRO.PlayerData.PlayerId });
                commandBuffer.DestroyEntity(reqEntity);
            }

            commandBuffer.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}