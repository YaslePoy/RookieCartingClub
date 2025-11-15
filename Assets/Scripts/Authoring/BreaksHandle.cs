using RookieCartingClub.Components;
using Unity.Entities;
using UnityEngine;

namespace RookieCartingClub.Authoring
{
    public class BreaksHandle : MonoBehaviour
    {
        public class RearWheelBaker : Baker<BreaksHandle>
        {
            public override void Bake(BreaksHandle authoring)
            {
                var e = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<RearWheel>(e);
            }
        }
    }
}