using Unity.Entities;

namespace RookieCartingClub.Components
{
    public struct EngineWheelData : IComponentData
    {
        public float Part;
        public Entity EngineResistant;
    }
}