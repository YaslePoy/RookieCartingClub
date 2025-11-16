using RookieCartingClub.Authoring;
using RookieCartingClub.Components;
using RookieCartingClub.Components.RPC;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace RookieCartingClub.Systems.Netcode.Server
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct GoInPitSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GoInPitRequest>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (reqSrc, rpc, reqEntity)
                     in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<GoInPitRequest>>().WithEntityAccess())
            {
                var id = rpc.ValueRO.PlayerId;

                foreach (var (cartData, cartEntity)
                         in SystemAPI.Query<RefRO<CartData>>().WithEntityAccess())
                {
                    if (cartData.ValueRO.PlayerId != id)
                        continue;

                    commandBuffer.SetComponentEnabled<TrackPlacementRequest>(cartEntity, true);
                    commandBuffer.SetComponent(cartEntity, new TrackPlacementRequest() { CollectionId = 1 });
                    Debug.Log($"GoInPit Request: {id}");

                    break;
                }

                Debug.Log($"GoInPit Request: {id}");

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