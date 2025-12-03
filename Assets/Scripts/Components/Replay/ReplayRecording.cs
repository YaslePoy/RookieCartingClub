using Unity.Entities;

namespace RookieCartingClub.Components.Replay
{
    public struct ReplayRecording : IComponentData
    {
        public RecordingState State;
    }

    public enum RecordingState : byte
    {
        None,
        Starting,
        Recording,
        Stopping
    }
}