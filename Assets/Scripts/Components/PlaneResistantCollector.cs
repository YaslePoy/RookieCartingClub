using Unity.Entities;

namespace RookieCartingClub.Components
{
    public struct PlaneResistantCollector : IComponentData
    {
        public float MaxForce;
        public float EfficiencyFactor;
        public Entity Collector;
    }
}