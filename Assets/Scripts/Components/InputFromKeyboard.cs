using Unity.Entities;

namespace RookieCartingClub.Components
{
    public struct InputFromKeyboard : IComponentData
    {
        public float MaxAngle;
        public float Sensetivity;
    }
}