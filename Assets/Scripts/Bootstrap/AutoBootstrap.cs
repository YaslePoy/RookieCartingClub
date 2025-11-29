using System;
using RookieCartingClub.Authoring;
using RookieCartingClub.Components;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

public class AutoBootstrap : ClientServerBootstrap
{
    public override bool Initialize(string defaultWorldName)
    {
        Debug.Log("Initializing AutoBootstrap");
        if (SessionSetup.RequestedSession is null)
        {
            return false;
        }
        switch (SessionSetup.RequestedSession)
        {
            case LocalSession:
                DefaultConnectAddress =  NetworkEndpoint.LoopbackIpv4;
                DefaultListenAddress =  NetworkEndpoint.AnyIpv4;
                CreateServerWorld("Server world");
                CreateClientWorld("Player world");
                Debug.Log("Worlds stated");
                break;
            case NetworkSession networkSession:
                DefaultConnectAddress = NetworkEndpoint.Parse(networkSession.Ip, networkSession.Port, NetworkFamily.Ipv4);
                AutoConnectPort = networkSession.Port;
                CreateClientWorld("Player world");
                Debug.Log("Connection initiated for port: " + networkSession.Port);
                break;
            case ServerSession serverConfig:
                DefaultListenAddress = NetworkEndpoint.AnyIpv4.WithPort(serverConfig.Port);
                AutoConnectPort = serverConfig.Port;
                CreateServerWorld("Server world");
                Debug.Log("Server started on port " + serverConfig.Port);
                break;
        }
        return true;
    }

    public void StopGameSession()
    {
        switch (SessionSetup.RequestedSession)
        {
            case LocalSession localSession:
                ClientWorld.Dispose();
                ServerWorld.Dispose();
                break;
            case NetworkSession networkSession:
                ClientWorld.Dispose();
                break;
            case ServerSession serverSession:
                ServerWorld.Dispose();
                break;
        }
    }
}