using System.Linq;
using RookieCartingClub.Authoring;
using RookieCartingClub.Ui;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace RookieCartingClub.Ui
{
    public class SelectorUIHandler : MonoBehaviour
    {
        private UIDocument _document;
        private string trackId = string.Empty;

        private void Start()
        {
            _document = gameObject.GetComponent<UIDocument>();
            _document.rootVisualElement.Q<Label>("NickName").text = SessionSetup.Nickname;
            var tracks = _document.rootVisualElement.Q<VisualElement>("CardsHolder").Children().ToList();
            foreach (var track in tracks)
            {
                track.RegisterCallback<MouseEnterEvent>(_ =>
                {
                    track.style.backgroundColor = new Color(0f, 0f, 0f, 0.1f);
                });
                track.RegisterCallback<MouseLeaveEvent>(_ =>
                {
                    if (trackId == track.name)
                    {
                        return;
                    }

                    track.style.backgroundColor = new Color { r = 0f, g = 0f, b = 0f, a = 0f };
                });

                track.RegisterCallback<MouseDownEvent>(_ =>
                {
                    foreach (var t in tracks)
                    {
                        t.style.backgroundColor = new Color { r = 0f, g = 0f, b = 0f, a = 0f };
                    }

                    track.style.backgroundColor = new Color(0f, 0f, 0f, 0.3f);
                    trackId = track.name;
                    UpdateGoingGame();
                });
            }
            
            _document.rootVisualElement.Q<Button>("MPButton").clicked += () =>
            {
                var networkSession = new NetworkSession { Ip = "95.105.78.72" };
                switch (trackId)
                {
                    case "ProScene":
                        networkSession.Port = 7777;
                        break;
                    case "RookieCircuit":
                        networkSession.Port = 7776;
                        break;
                }

                SessionSetup.SceneName = trackId;
                SessionSetup.RequestedSession = networkSession;
                ClientServerBootstrap.AutoConnectPort = networkSession.Port;
                var bootstrap = new AutoBootstrap();
                bootstrap.Initialize("");
                SceneManager.LoadScene($"Scenes/{trackId}");
            };

            _document.rootVisualElement.Q<Button>("SPButton").clicked += () =>
            {
                SessionSetup.SceneName = trackId;
                SessionSetup.RequestedSession = new LocalSession();
                ClientServerBootstrap.AutoConnectPort = 7772;
                var bootstrap = new AutoBootstrap();
                bootstrap.Initialize("");
                SceneManager.LoadScene($"Scenes/{trackId}");
            };
        }

        private void UpdateGoingGame()
        {
            var buttons = _document.rootVisualElement.Q<VisualElement>("Mods");
            if (string.IsNullOrWhiteSpace(LoginUIController.User.username) || string.IsNullOrWhiteSpace(trackId))
            {
                buttons.style.visibility = Visibility.Hidden;
            }
            else
            {
                buttons.style.visibility = Visibility.Visible;
            }
        }
    }
}