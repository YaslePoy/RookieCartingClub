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
            var cartPrefab = spawner.CartPrefab;

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            ecb.AddComponent<NetworkStreamInGame>(observerConnection);

            foreach (var initialRecordingCondition in _replay.InitialRecordingConditions)
            {
                var cartInstance = ecb.Instantiate(cartPrefab);

                ecb.SetComponent(cartInstance, initialRecordingCondition.Position);
                ecb.SetComponent(cartInstance, initialRecordingCondition.Velocity);
                ecb.SetComponent(cartInstance, new GhostOwner { NetworkId = 1 });
                ecb.AppendToBuffer(observerConnection, new LinkedEntityGroup { Value = cartInstance });
            }

            ecb.Playback(entityManager);

            state.Enabled = false;
            Debug.Log("Players initialized");
        }
    }
}