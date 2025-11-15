using System.IO;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerConfigLoader : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
#if !UNITY_EDITOR
        if (!File.Exists("config.json"))
        {
            print("No config file found");
            Application.Quit();
        }
        var path = "config.json";

#else
        var path = "/home/micial/projects/unity/build/server/config.json";
#endif


        var config = JsonUtility.FromJson<ServerConfig>(File.ReadAllText(path));

        SessionSetup.RequestedSession = new ServerSession
        {
            Port = config.Port,
            SessionTimetable = config.SessionTimetable
        };

        SceneManager.LoadScene(config.TrackId);
    }
}