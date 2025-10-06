using System;
using DefaultNamespace;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientSceneSetup : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var networkManager = GetComponent<NetworkManager>();
        var transport = GetComponent<UnityTransport>();
        var setup = SessionSetup.ReqeustedSession;
        print($"Loading {SceneManager.GetActiveScene().name}");
        switch (setup)
        {
            case LocalSession:
                transport.ConnectionData.Address = "127.0.0.1";
                networkManager.StartHost();
                break;
            case NetworkSession networkSession:
                transport.ConnectionData.Address = networkSession.Ip;
                transport.ConnectionData.Port = networkSession.Port;
                networkManager.StartClient();
                break;
            case ServerSession serverConfig:
                transport.ConnectionData.Port =  serverConfig.Port;
                networkManager.StartServer();
                
                print($"Server started on port {transport.ConnectionData.Port}");
                
                break;
        }
    }
    
    
}
