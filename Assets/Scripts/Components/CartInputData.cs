using Unity.Entities;

namespace RookieCartingClub.Components
{
    public struct CartInputData : IComponentData
    {
        public float CurrentAngle;
        public float CurrentEngine;
        public float CurrentBreaks;
        public bool AllowControl;
    }
}