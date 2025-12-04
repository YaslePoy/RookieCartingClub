using Unity.Entities;

namespace RookieCartingClub.Components
{
    public struct CameraData : IComponentData
    {
        public int PlayerIndex;
        public Entity PlayerEntity;
        public int ViewIndex;
    }
}