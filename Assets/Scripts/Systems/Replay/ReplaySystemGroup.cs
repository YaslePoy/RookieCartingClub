using Unity.Entities;
using Unity.NetCode;

namespace RookieCartingClub.Systems.Replay
{
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    [UpdateBefore(typeof(UIUpdateSystem))]
    public partial class ReplaySystemGroup : ComponentSystemGroup
    {
    }
}
