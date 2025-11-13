using System;
using Unity.Entities;
using UnityEngine;

//todo
public class PlaneResistant : MonoBehaviour
{
    public float MaxForce;
    public float K = 1;
    public CartWheelAuthoring Collector;
    private Rigidbody _rigidbody;
    private VelocityProvider _velocity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _rigidbody = GetComponentInParent<Rigidbody>();
        _velocity = GetComponent<VelocityProvider>();
        if (_velocity == null) _velocity = GetComponentInParent<VelocityProvider>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        // var velocity = _velocity.Velocity;
        //
        // if (velocity.sqrMagnitude == 0) return;
        //
        // var energy = velocity.sqrMagnitude * _rigidbody.mass / 2 * ForcePart;
        // var stopForce = energy / 0.1f;
        // var resistanceFactor = Vector3.Dot(velocity.normalized, Normal);
        // var forceVector = Normal;
        // if (resistanceFactor > 0) forceVector *= -1;
        //
        // var finalForce = forceVector *
        //                  Helpers.Min(Math.Abs(resistanceFactor * Friction * _rigidbody.mass * 10f * ForcePart) * K,
        //                      MaxForce, stopForce);
        //
        // _rigidbody.AddForceAtPosition(finalForce, transform.position);
    }

    private void OnTransformParentChanged()
    {
        Destroy(gameObject);
    }
}

public class PlaneResistantBaker : Baker<PlaneResistant>
{
    public override void Bake(PlaneResistant authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);

        var collector = new PlaneResistantCollector
        {
            MaxForce = authoring.MaxForce,
            K = authoring.K,
            Collector = GetEntity(authoring.Collector, TransformUsageFlags.Dynamic)
        };

        AddComponent(entity, collector);
    }
}

public struct PlaneResistantCollector : IComponentData
{
    public float MaxForce;
    public float K;
    public Entity Collector;
}