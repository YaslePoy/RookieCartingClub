using Unity.Entities;

namespace RookieCartingClub.Systems.Replay
{
    [UpdateInGroup(typeof(CartPhysicsSimulationGroup))]
    [UpdateBefore(typeof(InputPhysicalImplementationSystem))]
    public partial class ReplaySystemGroup : ComponentSystemGroup
    {
    }
}
