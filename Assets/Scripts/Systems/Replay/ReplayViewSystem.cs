using RookieCartingClub.Authoring;
using RookieCartingClub.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEngine.InputSystem;

namespace RookieCartingClub.Systems.Replay
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [UpdateBefore(typeof(UIUpdateSystem))]
    public partial class ReplayViewSystem : SystemBase
    {
        private InputAction _next;
        private InputAction _prev;
        private EntityQuery _playersQuery;
        private bool _game;
        protected override void OnCreate()
        {
            if (SessionSetup.RequestedSession is not ReplaySession)
            {
                _game = true;
            }

            _next = InputSystem.actions.FindAction("Next");
            _prev = InputSystem.actions.FindAction("Previous");

            var builder = new EntityQueryBuilder(Allocator.Temp).WithAll<CartData>();
            _playersQuery = GetEntityQuery(builder);
            RequireForUpdate<CameraData>();
        }

        protected override void OnUpdate()
        {
            var camera = SystemAPI.GetSingletonRW<CameraData>().ValueRW;
            var players = _playersQuery.ToEntityArray(Allocator.Temp);
            
            if (_next.WasPressedThisFrame()) 
                camera.PlayerIndex++;

            if (_prev.WasPressedThisFrame()) 
                camera.PlayerIndex--;
            
            camera.PlayerEntity = players[camera.PlayerIndex & players.Length];
        }
    }
}