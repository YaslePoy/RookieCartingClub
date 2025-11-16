using Unity.Entities;
using Unity.NetCode;

namespace RookieCartingClub.Components
{
    [GhostComponent(OwnerSendType = SendToOwnerType.All)]
    public struct CartInputData : IInputComponentData
    {
        [GhostField]
        public float CurrentAngle;
        [GhostField]
        public float CurrentEngine;
        [GhostField]
        public float CurrentBreaks;
        public bool AllowControl;
    }
}