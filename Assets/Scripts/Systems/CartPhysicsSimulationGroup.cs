using Unity.Entities;
using Unity.Physics.Systems;

namespace RookieCartingClub.Systems
{
    // [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(PhysicsInitializeGroup))]
    public partial class CartPhysicsSimulationGroup : ComponentSystemGroup
    {
    }
}