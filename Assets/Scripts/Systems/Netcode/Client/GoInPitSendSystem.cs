using RookieCartingClub.Components;
using RookieCartingClub.Components.RPC;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace RookieCartingClub.Systems.Netcode.Client
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    public partial struct GoInPitSendSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TrackPlacementRequest>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

            var connectionEntity = new Entity();

            foreach (var (_, entity) in SystemAPI.Query<RefRO<NetworkId>>().WithEntityAccess())
            {
                connectionEntity = entity;
            }

            foreach (var (localRequest, cartData)
                     in SystemAPI.Query<EnabledRefRW<TrackPlacementRequest>, RefRO<CartData>>())
            {
                localRequest.ValueRW = false;

                CreatePittingRequest(commandBuffer, cartData, connectionEntity);
            }

            commandBuffer.Playback(state.EntityManager);
        }

        private static void CreatePittingRequest(EntityCommandBuffer commandBuffer, RefRO<CartData> cartData, Entity connectionEntity)
        {
            var req = commandBuffer.CreateEntity();
            commandBuffer.AddComponent(req, new GoInPitRequest
            {
                PlayerId = cartData.ValueRO.PlayerId
            });
            commandBuffer.AddComponent(req, new SendRpcCommandRequest { TargetConnection = connectionEntity });
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}