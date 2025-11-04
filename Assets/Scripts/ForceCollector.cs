using Unity.Entities;
using UnityEngine;

public class ForceCollector : MonoBehaviour
{
    public float Friction;
}

public class ForceApplierBaker : Baker<ForceCollector>
{
    public override void Bake(ForceCollector authoring)
    {
        var e = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent<CartWheel>(e);
        AddBuffer<ForceApplyRequest>(e);
        AddComponent(e, new ForceCollectorData
        {
            Friction = authoring.Friction,
        });
    }
}

public struct ForceCollectorData : IComponentData
{
    public float Friction;
}