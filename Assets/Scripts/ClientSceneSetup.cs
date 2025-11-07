using System;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientSceneSetup : MonoBehaviour
{
    public RaceControl RaceControl;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        var networkManager = GetComponent<NetworkManager>();
        var transport = GetComponent<UnityTransport>();
        var setup = SessionSetup.RequestedSession;
        print($"Loading {SceneManager.GetActiveScene().name}");
        switch (setup)
        {
            case LocalSession:
                transport.ConnectionData.Address = "127.0.0.1";
                RaceControl.racePeriods =
                    new Queue<IRacePeriod>(new IRacePeriod[] { new PracticePeriod { Duration = 10 * 60 } });
                networkManager.StartHost();
                break;
            case NetworkSession networkSession:
                transport.ConnectionData.Address = networkSession.Ip;
                transport.ConnectionData.Port = networkSession.Port;
                networkManager.StartClient();
                break;
            case ServerSession serverConfig:
                transport.ConnectionData.Port = serverConfig.Port;
                if (networkManager.StartServer())
                {
                    print($"Server started on port {transport.ConnectionData.Port}");
                    RaceControl.racePeriods = ParseConfiguration(serverConfig.SessionTimetable);
                }
                else
                {
                    print("Failed to start server");
                }

                break;
        }
    }

    public Queue<IRacePeriod> ParseConfiguration(string configuration)
    {
        var queue = new Queue<IRacePeriod>();
        var parts = configuration.Split(' ');
        foreach (var part in parts)
        {
            var type = part.Split(':')[0];

            Debug.Log($"{type}: {part}");

            switch (type)
            {
                case "PRE":
                    queue.Enqueue(new PrePeriod { Duration = Convert.ToDouble(part.Split(':')[1]) });
                    break;
                case "RACE":
                    queue.Enqueue(new RacePeriod { Duration = Convert.ToDouble(part.Split(':')[1]) });
                    break;
                case "PRACTICE":
                    queue.Enqueue(new PracticePeriod { Duration = Convert.ToDouble(part.Split(':')[1]) });
                    break;
                case "FINISH":
                    queue.Enqueue(new FinishPeriod { Duration = Convert.ToDouble(part.Split(':')[1]) });
                    break;
            }
        }

        return queue;
    }
}