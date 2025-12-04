using RookieCartingClub.Authoring;
using RookieCartingClub.Components;
using RookieCartingClub.Components.Replay;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics.Systems;
using UnityEngine;

namespace RookieCartingClub.Systems.Replay
{
    [UpdateInGroup(typeof(CartPhysicsSimulationGroup))]
    [UpdateBefore(typeof(InputPhysicalImplementationSystem))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct ReplaySystem : ISystem
    {
        private EntityQuery _playersQuery;
        private Components.Replay.Replay _replay;

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

        [BurstCompile]
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
                inputBuffer.RemoveAt(0);
            }
        }
    }
}