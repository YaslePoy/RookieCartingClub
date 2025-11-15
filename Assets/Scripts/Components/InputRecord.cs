using Unity.Entities;

namespace RookieCartingClub.Components
{
    public struct InputRecord : IBufferElementData
    {
        public CartInputData Input;
    }
}