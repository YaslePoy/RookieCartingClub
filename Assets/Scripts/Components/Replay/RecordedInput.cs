using Unity.Entities;

namespace RookieCartingClub.Components.Replay
{
    public struct RecordedInput : IBufferElementData
    {
        public CartInputData Input;
    }
}