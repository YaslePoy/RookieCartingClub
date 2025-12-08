using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RookieCartingClub.Determ.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation | WorldSystemFilterFlags.LocalSimulation)]
    public partial class CubeRequest : SystemBase
    {
        private InputAction _spawn;
        private InputAction _despawn;

        protected override void OnCreate()
        {
            _spawn = InputSystem.actions.FindAction("Jump");
            _despawn = InputSystem.actions.FindAction("Attack");
            RequireForUpdate<NetworkId>();
        }  

        protected override void OnUpdate()
        {
            if (_spawn.IsPressed())
            {
                SendRequest(CubeEventType.SpawnCube);
            }

            if (_despawn.WasPressedThisFrame())
            {
                SendRequest(CubeEventType.RemoveCube);
            }
        }

        private void SendRequest(CubeEventType spawnCube)
        {
            var connection = Entity.Null;
            foreach (var (_, connectionEntity)
                     in SystemAPI.Query<RefRO<NetworkId>>().WithEntityAccess().WithNone<NetworkStreamInGame>())
            {
                connection = connectionEntity;
            }
            
            EntityManager.AddComponent<NetworkStreamInGame>(connection);
            var req = EntityManager.CreateEntity();
            EntityManager.AddComponentData(req, new SendRpcCommandRequest { TargetConnection = connection });
            EntityManager.AddComponentData(req, new CubeRpc{ Type = spawnCube });
            
            
        }
    }
}