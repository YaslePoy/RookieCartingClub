using Unity.Entities;

namespace RookieCartingClub.Components
{
    public struct CartSpawner :  IComponentData
    {
        public Entity CartPrefab;
        public Entity ReplayCartPrefab;
    }
}