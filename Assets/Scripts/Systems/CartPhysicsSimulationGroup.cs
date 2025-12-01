using Unity.Entities;
using Unity.Physics.Systems;

namespace RookieCartingClub.Systems
{
    [UpdateInGroup(typeof(PhysicsInitializeGroup))]
    public partial class CartPhysicsSimulationGroup : ComponentSystemGroup
    {
    }
}