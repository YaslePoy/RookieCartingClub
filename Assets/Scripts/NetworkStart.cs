using Unity.Netcode;
using UnityEngine;

public class NetworkStart : MonoBehaviour
{
    public NetworkMode Mode;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var manager = GetComponent<NetworkManager>();
        switch (Mode)
        {
            case NetworkMode.Server:
                manager.StartServer();
                break;
            case NetworkMode.Client:
                manager.StartClient();
                break;
            case NetworkMode.Host:
                manager.StartHost();
                break;
        }
    }
}

public enum NetworkMode
{
    Server,
    Client,
    Host
}
