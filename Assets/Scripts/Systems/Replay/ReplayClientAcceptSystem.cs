using RookieCartingClub.Authoring;
using RookieCartingClub.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace RookieCartingClub.Systems.Replay
{
    public partial struct ReplayClientAcceptSystem : ISystem
    {
        private EntityQuery _cartQuery;
        public void OnCreate(ref SystemState state)
        {
            if (SessionSetup.RequestedSession is not ReplaySession)
            {
                state.Enabled = false;
                return;
            }
            state.RequireForUpdate<NetworkStreamDriver>();
            state.RequireForUpdate<NetworkStreamConnection>();
            _cartQuery = SystemAPI.QueryBuilder().WithAll<CartInputData>().Build();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            
            Entity observerConnection = SystemAPI.GetSingletonEntity<NetworkStreamConnection>();
            var driver = SystemAPI.GetSingleton<NetworkStreamDriver>();
            var conn = driver.GetConnectionState(
                state.EntityManager.GetComponentData<NetworkStreamConnection>(observerConnection));
            
            Debug.Log($"{conn}");

            var buffer = entityManager.GetBuffer<LinkedEntityGroup>(observerConnection);
            
            foreach (var cart in _cartQuery.ToEntityArray(Allocator.Temp))
            {
                buffer.Add(new LinkedEntityGroup
                {
                    Value = cart
                });
            }
            Debug.Log("Connected");

            state.Enabled = false;
        }

    }
}