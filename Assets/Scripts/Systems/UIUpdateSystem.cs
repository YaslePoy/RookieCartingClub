using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial class UIUpdateSystem : SystemBase
{
    protected override void OnCreate()
    {
        CheckedStateRef.RequireForUpdate<CartData>();
    }

    protected override void OnUpdate()
    {
        var cart = SystemAPI.GetSingletonEntity<CartData>();
        var velocity = SystemAPI.GetComponent<PhysicsVelocity>(cart);
        UI.Instance.VelocityProvider = new ConstantVelocityProvider { Velocity = velocity.Linear };

        if (UI.Instance.InPitRequest) 
            SendInPitRequest();

        UI.Instance.UpdateUI();
        MapHandle.Instance.CartPosition = SystemAPI.GetComponent<LocalToWorld>(cart).Position;
        MapHandle.Instance.MoveSelf();
    }

    private void SendInPitRequest()
    {
        var id = UI.Instance.Cart.PlayerId;

        var replaceEntity = Entity.Null;

        foreach (var (data, entity) in SystemAPI.Query<RefRO<CartData>>().WithEntityAccess())
            if (data.ValueRO.PlayerId == id)
            {
                replaceEntity = entity;
                break;
            }

        EntityManager.SetComponentEnabled<TrackPlacementRequest>(replaceEntity, true);
        EntityManager.SetComponentData(replaceEntity, new TrackPlacementRequest { CollectionId = 1 });
    }
}

public class ConstantVelocityProvider : IVelocityProvider
{
    public Vector3 Velocity { get; set; }
}

public interface IVelocityProvider
{
    Vector3 Velocity { get; }
}