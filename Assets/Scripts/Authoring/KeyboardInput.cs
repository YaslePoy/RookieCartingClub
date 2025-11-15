using RookieCartingClub.Components;
using Unity.Entities;
using UnityEngine;

namespace RookieCartingClub.Authoring
{
    public class KeyboardInput : MonoBehaviour
    {
        public float MaxAngle;
        public float Sensetivity;
    
        public class KeyboardInputBaker : Baker<KeyboardInput>
        {
            public override void Bake(KeyboardInput authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new InputFromKeyboard
                {
                    MaxAngle = authoring.MaxAngle,
                    Sensetivity = authoring.Sensetivity
                });
            }
        }
    }
}