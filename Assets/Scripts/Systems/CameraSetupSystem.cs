using RookieCartingClub.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace RookieCartingClub.Systems
{
    public partial struct CameraSetupSystem : ISystem
    {
        private EntityQuery _playerFindQuery;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _playerFindQuery = SystemAPI.QueryBuilder().WithAll<GhostOwnerIsLocal, CartData>().Build();
            state.RequireForUpdate(_playerFindQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (_playerFindQuery.IsEmpty)
                return;
            
            var player = _playerFindQuery.ToEntityArray(Allocator.Temp)[0];

            var camera = default(CameraData);
            
            if (SystemAPI.HasSingleton<CameraData>() == false)
            {
                state.EntityManager.CreateSingleton<CameraData>();
            }
            else
            {
                camera = SystemAPI.GetSingleton<CameraData>();
            }

            camera.PlayerEntity = player;
            SystemAPI.SetSingleton(camera);
        }
    }
}