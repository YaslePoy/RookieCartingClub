using System;
using UnityEngine;

public class EngineWheel : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float Part;
    private Engine _engine;
    private Rigidbody _rigidbody;
    void Start()
    {
        _engine = GetComponentInParent<Engine>();
        _rigidbody = GetComponentInParent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _rigidbody.AddForce(transform.forward * (Part * _engine.CurrentForce));
        print($"Wheel force: {_engine.CurrentForce}");
        
    }

    private void OnTransformParentChanged()
    {
        Destroy(this);
    }
}
