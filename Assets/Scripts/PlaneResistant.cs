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

    public float ForcePart;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponentInParent<Rigidbody>();
        Debug.Log($"Plane of {name}: {Normal}");
    }

    // Update is called once per frame
    void Update()
    {
        var velocity = _rigidbody.linearVelocity;
        if (velocity.sqrMagnitude == 0)
        {
            return;
        }

        if (velocity.magnitude > 10)
        {
            print("High speed");
        }
        
        
        float energy = MathF.Pow(_rigidbody.linearVelocity.magnitude, 2) * _rigidbody.mass / 2 * ForcePart;
        
        var resistanceFactor = Vector3.Dot(velocity.normalized,  Normal);
        var forceVector = Normal;
        if (resistanceFactor > 0)
        {
            forceVector *= -1;
        }
        print(resistanceFactor);
        _rigidbody.AddForce(forceVector * MathF.Min(Math.Abs(resistanceFactor * Friction * _rigidbody.mass * 10f * ForcePart), energy), ForceMode.Force);
    }
}
