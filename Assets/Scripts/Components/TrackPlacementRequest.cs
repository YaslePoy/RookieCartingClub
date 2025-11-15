using Unity.Entities;

namespace RookieCartingClub.Components
{
    public struct  TrackPlacementRequest : IComponentData, IEnableableComponent
    { 
        public int CollectionId;
    }
}