using Unity.Entities;

namespace RookieCartingClub.Components
{
    public struct InputFromWheel : IComponentData
    {
        public float WheelDegrees;
        public double SteerMultiplier;
    }
}