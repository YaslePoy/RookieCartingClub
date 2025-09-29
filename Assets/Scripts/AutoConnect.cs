using Unity.Netcode;
using UnityEngine;

public class AutoConnect : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var requirements = GetComponents<NetworkStart>();
        var Mode = requirements.Length == 1 ? requirements[0].Mode : NetworkMode.Host;
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
