using RookieCartingClub.Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

namespace RookieCartingClub.Systems.Netcode.Client
{
    [WorldSystemFilter(WorldSystemFilterFlags.LocalSimulation | WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation, WorldSystemFilterFlags.LocalSimulation | WorldSystemFilterFlags.ClientSimulation)]
    public partial struct DisconnectionRegisterSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
           state.RequireForUpdate<NetworkStreamDriver>();
        }

        // [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var driver = SystemAPI.GetSingletonRW<NetworkStreamDriver>();


            var events = driver.ValueRO.ConnectionEventsForTick;

            var requireReconnection = false;
            
            foreach (var networkEvent in events)
            {
                if (networkEvent.State == ConnectionState.State.Disconnected)
                {
                    requireReconnection = true;
                    break;
                }

            }

            if (!requireReconnection)
                return;
            
            
            Debug.Log("Disconnected!!!!");
            Debug.Log("Try to reconnect");
            
            var connectRequest = state.EntityManager.CreateEntity(typeof(NetworkStreamRequestConnect));
            var session = SessionSetup.RequestedSession as NetworkSession;
            var endpoint = NetworkEndpoint.Parse(session.Ip, session.Port, NetworkFamily.Ipv4);
            state.EntityManager.SetComponentData(connectRequest, new NetworkStreamRequestConnect { Endpoint =  endpoint});
        }
    }
}