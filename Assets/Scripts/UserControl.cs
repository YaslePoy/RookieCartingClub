using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class UserControl : MonoBehaviour
{
    private InputAction _forceAction;
    private Rigidbody _rigidbody;

    public float Angle;
    public float MaxAngle;
    public float Sensetivity;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _forceAction = InputSystem.actions.FindAction("Move");
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        var movement = _forceAction.ReadValue<Vector2>();
        if ( movement.y != 0)
        {
            Debug.Log($"Adding force: {movement}");
            _rigidbody.AddForce(transform.forward * (movement.y * 100));
        }

        if (movement.x != 0)
        {
            Angle = MathF.Min(MaxAngle, Math.Abs(Angle + movement.x * Sensetivity *  Time.deltaTime)) * (MathF.Sign(Angle) == 0 ? movement.x : MathF.Sign(Angle));
        }
        else
        {
            if (Angle != 0)
            {
                Angle -= MathF.Min(MathF.Abs(Angle), Sensetivity * 2 * Time.deltaTime) * MathF.Sign(Angle);
            }
        }
    }
}
