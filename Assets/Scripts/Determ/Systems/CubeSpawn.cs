using RookieCartingClub.Components;
using RookieCartingClub.Components.RPC;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace RookieCartingClub.Determ.Systems
{
    public partial struct CubeSpawn : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CubeSpawner>();
            var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<CubeRpc>()
                .WithAll<ReceiveRpcCommandRequest>();
            state.RequireForUpdate(state.GetEntityQuery(builder));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var cartPrefab = SystemAPI.GetSingleton<CubeSpawner>().PrefabEntity;

            foreach (var (reqSrc, rpc, reqEntity) in SystemAPI
                         .Query<RefRO<ReceiveRpcCommandRequest>, RefRO<CubeRpc>>()
                         .WithEntityAccess())
            {
               var newCube = state.EntityManager.Instantiate(cartPrefab);
               state.EntityManager.GetBuffer<LinkedEntityGroup>(reqSrc.ValueRO.SourceConnection).Add(
                   new LinkedEntityGroup
                   {
                       Value = newCube,
                   });
               
               
               state.EntityManager.DestroyEntity(reqEntity);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}