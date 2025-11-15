using RookieCartingClub.Components;
using Unity.Entities;
using UnityEngine;

namespace RookieCartingClub.Authoring
{
    public class CartWheelAuthoring : MonoBehaviour
    {
        public float MaxResistance;
        public float ForcePart;
        public float Friction;

        public class CartWheelBaker : Baker<CartWheelAuthoring>
        {
            public override void Bake(CartWheelAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity,
                    new CartWheel
                    {
                        MaxResistance = authoring.MaxResistance, ForcePart = authoring.ForcePart,
                        Mass = authoring.GetComponentInParent<Rigidbody>().mass, Friction = authoring.Friction
                    });
                AddBuffer<ForceApplyRequest>(entity);
            }
        }
    }
}