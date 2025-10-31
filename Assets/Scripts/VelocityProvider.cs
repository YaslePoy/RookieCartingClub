using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class VelocityProvider : MonoBehaviour
{
    public Vector3 Velocity => _velocity;
    private Vector3 _velocity;
    private Vector3 _lastPosition;

    void Start()
    {
        _velocity = Vector3.zero;
        _lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        var currentPosition = transform.position;
        var newVel = (currentPosition - _lastPosition) / Time.fixedDeltaTime;

        _velocity = newVel;
        _lastPosition = currentPosition;
    }
}

public class VelocityProviderBaker : Baker<VelocityProvider>
{
    public override void Bake(VelocityProvider authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent<LocalVelocity>(entity);
    }
}

public struct LocalVelocity : IComponentData
{
    public float3 Velocity;
}