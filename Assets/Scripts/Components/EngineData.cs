using Unity.Entities;

namespace RookieCartingClub.Components
{
    public struct EngineData : IComponentData
    {
        public float MaxForce;
        public float MaxSpeed;
        public float CurrentForce;
    }
}