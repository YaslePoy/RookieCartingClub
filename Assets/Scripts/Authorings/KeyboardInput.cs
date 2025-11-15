using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

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