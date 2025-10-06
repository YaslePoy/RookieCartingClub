using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace DefaultNamespace
{
    public class SelectorUIHandler : MonoBehaviour
    {
        private UIDocument  _document;
        private string trackId = string.Empty;
        private TextField NickTF; 
        private void Start()
        {
            _document = gameObject.GetComponent<UIDocument>();
            var tracks = _document.rootVisualElement.Q<VisualElement>("CardsHolder").Children().ToList();
            foreach (var track in tracks)
            {
                track.RegisterCallback<MouseEnterEvent>(_ =>
                {
                    track.style.backgroundColor = new Color(0f, 0f, 0f, 0.1f);
                });
                track.RegisterCallback<MouseLeaveEvent>(_ =>
                {
                    if (trackId== track.name)
                    {
                        return;
                    }
                    track.style.backgroundColor = new Color { r = 0f, g = 0f, b = 0f, a = 0f};
                });
                
                track.RegisterCallback<MouseDownEvent>(_ =>
                {
                    foreach (var t in tracks)    
                    {
                        t.style.backgroundColor = new Color { r = 0f, g = 0f, b = 0f, a = 0f};
                    }
                    track.style.backgroundColor = new Color(0f, 0f, 0f, 0.3f);
                    trackId = track.name;
                    _document.rootVisualElement.Q<VisualElement>("Mods").style.visibility = Visibility.Visible;
                });
            }
            
            NickTF = _document.rootVisualElement.Q<TextField>("NickTF");

            _document.rootVisualElement.Q<Button>("MPButton").clicked += () =>
            {
                var networkSession = new NetworkSession { Ip = "95.105.78.72" };
                switch (trackId)
                {
                    case "ProCircuit":
                        networkSession.Port = 7777;
                        break;
                    case "RookieCircuit":
                        networkSession.Port = 7776;
                        break;
                }
                
                SessionSetup.ReqeustedSession = networkSession;
                SceneManager.LoadScene($"Scenes/{trackId}");
            };

            _document.rootVisualElement.Q<Button>("SPButton").clicked += () =>
            {
                SessionSetup.ReqeustedSession = new LocalSession();
                SceneManager.LoadScene($"Scenes/{trackId}");
            };
        }
    }
}