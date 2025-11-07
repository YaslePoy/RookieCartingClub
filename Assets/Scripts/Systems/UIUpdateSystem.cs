using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial class UIUpdateSystem : SystemBase
{
    protected override void OnCreate()
    {
    }

    protected override void OnUpdate()
    {
        if (!SystemAPI.TryGetSingletonEntity<CartData>(out var cart))
            return;

        var velocity = SystemAPI.GetComponent<PhysicsVelocity>(cart);
        UI.Instance.VelocityProvider = new ConstantVelocityProvider { Velocity = velocity.Linear };
        UI.Instance.UpdateUI();
        MapHandle.Instance.CartPosition = SystemAPI.GetComponent<LocalToWorld>(cart).Position;
        MapHandle.Instance.MoveSelf();
    }
}

public class ConstantVelocityProvider : IVelocityProvider
{
    public Vector3 Velocity { get; set; }
}