using RookieCartingClub.Components;
using RookieCartingClub.Components.Replay;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace RookieCartingClub.Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(GhostSimulationSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation |
                       WorldSystemFilterFlags.LocalSimulation)]
    public partial struct CartOnTrackUpdateSystem : ISystem
    {
        private EntityQuery _playerQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
            state.RequireForUpdate<NetworkStreamInGame>();


            var playerBuilder = new EntityQueryBuilder(Allocator.Temp).WithAll<CartData, GhostOwnerIsLocal>();
            _playerQuery = state.GetEntityQuery(playerBuilder);
        }

        public void OnUpdate(ref SystemState state)
        {
            var requirePlayback = SystemAPI.HasSingleton<ReplayPlayback>();
            if (requirePlayback == false)
                return;

            var hasPlayer = _playerQuery.IsEmpty == false;

            if (hasPlayer == false)
            {
                var cartIndicatorEntity = SystemAPI.GetSingletonEntity<CartOnTrack>();
                state.EntityManager.RemoveComponent<CartOnTrack>(cartIndicatorEntity);

                return;
            }

            Debug.Log("Setting up player for camera");

            if (SystemAPI.HasSingleton<CameraData>() == false)
            {
                state.EntityManager.CreateSingleton<CameraData>();
            }

            var cameraData = SystemAPI.GetSingletonRW<CameraData>();
            cameraData.ValueRW.PlayerEntity = _playerQuery.ToEntityArray(Allocator.Temp)[0];

            var indicatorEntity = _indicatorQuery.GetSingletonEntity();
            state.EntityManager.AddComponent<CartOnTrack>(indicatorEntity);
        }
    }
}