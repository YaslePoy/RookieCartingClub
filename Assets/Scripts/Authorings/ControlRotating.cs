using Unity.Entities;
using Unity.Mathematics;
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


public struct FrontWheel : IComponentData
{
}

public struct RearWheel : IComponentData
{
}

public struct ForceApplyRequest : IBufferElementData
{
    public float3 Force;
}