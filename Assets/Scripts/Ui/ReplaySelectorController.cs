using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using RookieCartingClub.Authoring;
using RookieCartingClub.Components.Replay;
using RookieCartingClub.Systems.Replay;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace RookieCartingClub.Ui
{
    public class ReplaySelectorController : MonoBehaviour
    {
        private readonly Dictionary<string, string> _trackMapCasts = new() { { "ProScene", "pro_scheme" } };
        private UIDocument _doc;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _doc = GetComponent<UIDocument>();
            var listView = _doc.rootVisualElement.Q<ListView>("ListView");
            var replays = GetReplays();

            _doc.rootVisualElement.Q<Button>("Back").clicked += () =>
            {
                SceneManager.LoadScene("Scenes/SelectorScene");
            };

            var play = _doc.rootVisualElement.Q<Button>("Play");

            listView.selectedIndicesChanged += ids =>
            {
                foreach (var replay in replays)
                {
                    play.SetEnabled(true);
                    play.clicked += () =>
                    {
                        Debug.Log(replay.FileName);
                        GotoReplay(replay);
                    };
                    return;
                }
            };
            listView.itemsSource = replays;
            listView.makeItem += () => listView.itemTemplate.CloneTree();
            listView.bindItem = (item, index) => item.dataSource = replays[index];
        }

        private void GotoReplay(ReplayViewModel replayVm)
        {
            var file = File.ReadAllBytes(replayVm.FileName);
            var replay = Replay.Decode(file);

            SessionSetup.RequestedSession = new ReplaySession()
            {
                ReplayData = replay,
            };
            
            
            SceneManager.LoadScene($"Scenes/{replayVm.TrackName}");
            new AutoBootstrap().Initialize("");
        }

        private List<ReplayViewModel> GetReplays()
        {
            var replays = new List<ReplayViewModel>();

            var directory = Directory.GetCurrentDirectory();

            foreach (var file in Directory.GetFiles(directory, "*.rir"))
            {
                var replay = GetReplayFromFile(new FileInfo(file));
                replays.Add(replay);
            }

            return replays;
        }

        private ReplayViewModel GetReplayFromFile(FileInfo file)
        {
            using var stream = file.OpenRead();
            var headerSize = Marshal.SizeOf(typeof(ReplayRecordSystem.ReplayHeader));
            Span<byte> span = stackalloc byte[headerSize];
            var header = stream.Read(span);

            var casted = MemoryMarshal.Read<ReplayRecordSystem.ReplayHeader>(span);
            var frameSize = RecordedInput.Size;
            var replayTime = TimeSpan.FromSeconds(file.Length / frameSize / 60).ToString(@"mm\:ss");

            var texture = Resources.Load(_trackMapCasts[casted.TrackName.Value]) as Texture2D;
            var vm = new ReplayViewModel
            {
                FileName = file.Name,
                TrackName = casted.TrackName.Value,
                Duration = replayTime,
                TrackImage = texture
            };
            return vm;
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}