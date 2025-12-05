using RookieCartingClub.Authoring;
using RookieCartingClub.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace RookieCartingClub.Systems.Replay
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateBefore(typeof(ReplaySystem))]
    public partial struct ReplayCartSpawnSystem : ISystem
    {
        private Components.Replay.Replay _replay;

        public void OnCreate(ref SystemState state)
        {
            if (SessionSetup.RequestedSession is not ReplaySession replaySession)
            {
                state.Enabled = false;
                return;
            }

            _replay = replaySession.ReplayData;
            state.RequireForUpdate<CartSpawner>();
            state.RequireForUpdate<NetworkStreamConnection>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            Entity observerConnection = SystemAPI.GetSingletonEntity<NetworkStreamConnection>();
            var driver = SystemAPI.GetSingleton<NetworkStreamDriver>();
            var conn = driver.GetConnectionState(
                state.EntityManager.GetComponentData<NetworkStreamConnection>(observerConnection));
            Debug.Log($"{conn}");
            var entityManager = state.EntityManager;

            Debug.Log("Initializing players");
            var spawner = SystemAPI.GetSingleton<CartSpawner>();
            var cartPrefab = spawner.ReplayCartPrefab;


            entityManager.AddComponent<NetworkStreamInGame>(observerConnection);

            foreach (var initialRecordingCondition in _replay.InitialRecordingConditions)
            {
                var cartInstance = entityManager.Instantiate(cartPrefab);
                entityManager.GetBuffer<LinkedEntityGroup>(observerConnection).Add(new LinkedEntityGroup { Value = cartInstance });
            }

            state.Enabled = false;
            Debug.Log("Players initialized");
        }
    }
}