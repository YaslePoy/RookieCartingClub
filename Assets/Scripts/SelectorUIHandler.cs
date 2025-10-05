using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace DefaultNamespace
{
    public class SelectorUIHandler : MonoBehaviour
    {
        private UIDocument  _document;
        private int trackIndex = -1;
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
                    if (trackIndex == tracks.IndexOf(track))
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
                    trackIndex = tracks.IndexOf(track);
                    _document.rootVisualElement.Q<VisualElement>("Mods").style.visibility = Visibility.Visible;
                });
            }
            
            NickTF = _document.rootVisualElement.Q<TextField>("NickTF");
        }
    }
}