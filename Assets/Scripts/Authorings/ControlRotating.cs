using Unity.Entities;
using UnityEngine;

public class ControlRotating : MonoBehaviour
{
    public class ForwardWheelBaker : Baker<ControlRotating>
    {
        public override void Bake(ControlRotating authoring)
        {
            var e = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<FrontWheel>(e);
        }
    }
}