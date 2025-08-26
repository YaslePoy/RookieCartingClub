using System;
using Unity.Mathematics;
using UnityEngine;
using Plane = System.Numerics.Plane;

public class PlaneResistant : MonoBehaviour
{
    public Transform NormalTransform;
    public Vector3 Normal => NormalTransform.right;
    public float MaxResistance;
    public float Friction;
    private Rigidbody _rigidbody;
    private VelocityProvider _velocity;
    public float ForcePart;
    public float K = 1;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponentInParent<Rigidbody>();
        _velocity = GetComponent<VelocityProvider>();
        if (_velocity == null)
        {
            _velocity = GetComponentInParent<VelocityProvider>();
        }
        Debug.Log($"Plane of {name}: {Normal}");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var velocity = _velocity.Velocity;
        if (velocity.sqrMagnitude == 0)
        {
            return;
        }
        
        
        float energy = MathF.Pow(velocity.magnitude, 2) * _rigidbody.mass / 2 * ForcePart;
        
        var resistanceFactor = Vector3.Dot(velocity.normalized,  Normal);
        var forceVector = Normal;
        if (resistanceFactor > 0)
        {
            forceVector *= -1;
        }
        
        var finalForce = forceVector * (MathF.Min(Math.Abs(resistanceFactor * Friction * _rigidbody.mass * 10f * ForcePart), energy) * K);

        if (finalForce.magnitude > 10)
        {
            print($"{name} : {finalForce.magnitude} resistance");
        }
        
        Debug.DrawLine(transform.position, transform.position + finalForce.normalized, Color.red, 0.1f);
        Debug.DrawLine(transform.position, transform.position + velocity.normalized, Color.black, 0.1f);
        _rigidbody.AddForceAtPosition(finalForce, transform.position);
    }
}
