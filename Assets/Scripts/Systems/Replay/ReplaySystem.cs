using RookieCartingClub.Authoring;
using RookieCartingClub.Components;
using RookieCartingClub.Components.Replay;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace RookieCartingClub.Systems.Replay
{
    [UpdateInGroup(typeof(CartPhysicsSimulationGroup))]
    [UpdateBefore(typeof(InputPhysicalImplementationSystem))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct ReplaySystem : ISystem
    {
        public static double LastTime;
        private EntityQuery _playersQuery;
        private Components.Replay.Replay _replay;
        private bool _initialized;
        public void OnCreate(ref SystemState state)
        {
            if (SessionSetup.RequestedSession is not ReplaySession replaySession)
            {
                state.Enabled = false;
                return;
            }

            _replay = replaySession.ReplayData;

            state.EntityManager.CreateSingleton<ReplayPlayback>();
            state.RequireForUpdate<ReplayPlayback>();
            var builder = new EntityQueryBuilder(Allocator.Temp).WithNone<Prefab>().WithAll<CartInputData>();
            _playersQuery = state.GetEntityQuery(builder);
            state.RequireForUpdate<CartInputData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;


            var carts = _playersQuery.ToEntityArray(Allocator.Temp);
            for (var index = 0; index < _replay.Inputs.Length; index++)
            {
                Debug.Log("Applying input");
                var inputBuffer = _replay.Inputs[index];
                var cart = carts[index];

                if (inputBuffer.IsEmpty)
                {
                    entityManager.DestroyEntity(cart);
                    return;
                }

                entityManager.SetComponentData(cart, inputBuffer[0].Input);
                var position = entityManager.GetComponentData<LocalTransform>(cart);
                Debug.Log($"{position}");
                if (_initialized == false)
                {
                    entityManager.SetComponentData(cart, _replay.InitialRecordingConditions[index].Position);
                    entityManager.SetComponentData(cart, _replay.InitialRecordingConditions[index].Velocity);
                }
                
                inputBuffer.RemoveAt(0);
            }
            
            var now = SystemAPI.Time.ElapsedTime;

            Debug.Log($"Tick rate: {1f / (now - LastTime)}");
            LastTime = now;
            _initialized = true;
        }
    }
}