using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RookieCartingClub.Authoring
{
    public class ServerConfigLoader : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            return;
#if !UNITY_EDITOR
        if (!File.Exists("config.json"))
        {
            print("No config file found");
            Application.Quit();
        }
        var path = "config.json";

#else
            var path = @"C:\Users\Mimm\projects\unity\build\rcc_server\config.json";
#endif


            var config = JsonUtility.FromJson<ServerConfig>(File.ReadAllText(path));

            SessionSetup.RequestedSession = new ServerSession
            {
                Port = config.Port,
                SessionTimetable = config.SessionTimetable
            };

            var bootstrap = new AutoBootstrap();
            bootstrap.Initialize("");
            SceneManager.LoadScene(config.TrackId);
        }
    }
}