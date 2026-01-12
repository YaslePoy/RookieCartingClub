using RookieCartingClub.Components;
using Unity.Entities;
using UnityEngine;

namespace RookieCartingClub.Authoring
{
    public class WheelInput : MonoBehaviour
    {
        public float WheelDegrees;
        public float SteerMultiplier;
        private class WheelInputBaker : Baker<WheelInput>
        {
            public override void Bake(WheelInput authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new InputFromWheel
                {
                    WheelDegrees = authoring.WheelDegrees,
                    SteerMultiplier = authoring.SteerMultiplier,
                });
            }
        }

    }
}
