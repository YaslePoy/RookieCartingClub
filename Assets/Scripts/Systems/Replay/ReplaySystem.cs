using RookieCartingClub.Authoring;
using RookieCartingClub.Components;
using RookieCartingClub.Components.Replay;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace RookieCartingClub.Systems.Replay
{
    [UpdateInGroup(typeof(CartPhysicsSimulationGroup))]
    [UpdateBefore(typeof(InputPhysicalImplementationSystem))]
    public partial struct ReplaySystem : ISystem
    {
        private NativeReference<Components.Replay.Replay> _replay;
        private NativeReference<bool> _initialized;
        private EntityQuery _playersQuery;
        public void OnCreate(ref SystemState state)
        {
            if (SessionSetup.RequestedSession is not ReplaySession replaySession)
            {
                state.Enabled = false;
                return;
            }
            
            _replay = new NativeReference<Components.Replay.Replay>(replaySession.ReplayData, Allocator.Persistent);
            _initialized = new NativeReference<bool>(false, Allocator.Persistent);
            
            state.RequireForUpdate<RecordedInput>();
            state.RequireForUpdate<ReplayPlayback>();
            state.RequireForUpdate<CartInputData>();
            state.RequireForUpdate<CartSpawner>();
            var builder = new EntityQueryBuilder(Allocator.Temp).WithNone<Prefab>().WithAll<CartInputData>();
            _playersQuery = state.GetEntityQuery(builder);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var requireSpawn = _initialized.Value == false;
            
            var entityManager = state.EntityManager;
            
            if (requireSpawn)
            {
                var spawner = SystemAPI.GetSingleton<CartSpawner>();
                var cartPrefab = spawner.CartPrefab;
                
                foreach (var initialRecordingCondition in _replay.Value.InitialRecordingConditions)
                {
                    var cartInstance = entityManager.Instantiate(cartPrefab);
                    entityManager.SetComponentData(cartInstance, initialRecordingCondition.Position);
                    entityManager.SetComponentData(cartInstance, initialRecordingCondition.Velocity);
                }
                
                return;
            }

            var carts = _playersQuery.ToEntityArray(Allocator.Temp);
            for (var index = 0; index < _replay.Value.Inputs.Length; index++)
            {
                var inputBuffer = _replay.Value.Inputs[index];
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