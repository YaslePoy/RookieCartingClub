using Unity.Entities;

namespace RookieCartingClub.Components
{
    public struct ConnectRequest : IComponentData
    {
        public CartData PlayerData;
    }
}