using System;
using UnityEngine;

public class EngineWheel : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float Part;
    private Engine _engine;
    private Rigidbody _rigidbody;
    public PlaneResistant EngineResistant;
    void Start()
    {
        _engine = GetComponentInParent<Engine>();
        _rigidbody = GetComponentInParent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _rigidbody.AddForce(transform.forward * (Part * _engine.CurrentForce));
        if (_engine.CurrentForce == 0)
        {
            EngineResistant.K = 1;
        }
    }
}
