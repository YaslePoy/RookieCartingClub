using RookieCartingClub.Components;
using Unity.Entities;
using UnityEngine;

namespace RookieCartingClub.Authoring
{
    public class VelocityProvider : MonoBehaviour
    {
        public int Index;
        public class VelocityProviderBaker : Baker<VelocityProvider>
        {
            public override void Bake(VelocityProvider authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new LocalVelocity { Index = authoring.Index });
            }
        }
    }
}