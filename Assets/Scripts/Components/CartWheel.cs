using Unity.Entities;

namespace RookieCartingClub.Components
{
    public struct CartWheel : IComponentData
    {
        public float MaxResistance;
        public float ForcePart;
        public float Mass;
        public float Friction;
    }
}