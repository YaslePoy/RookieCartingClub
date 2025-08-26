using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class UserControl : MonoBehaviour
{
    private InputAction _forceAction;
    private InputAction _rotateAction;
    private Rigidbody _rigidbody;
    public bool AutoCenter = true;
    public float Angle;
    public float MaxAngle;
    public float Sensetivity;

    public float Engine;

    public float Breaks;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _forceAction = InputSystem.actions.FindAction("Move");
        _rotateAction = InputSystem.actions.FindAction("Jump");
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!AutoCenter)
            return;
        var movement = _forceAction.ReadValue<Vector2>();
        
        Engine = 0;
        Breaks = 0;
        
        if ( movement.y != 0)
        {
            Engine = 0;
            Breaks = 0;
            if (movement.y > 0)
            {
                Engine = movement.y;
            }
            else
            {
                Breaks = -movement.y;
            }
            // _rigidbody.AddForce(transform.forward * (movement.y * 100));
        }

        if (_rotateAction.WasPressedThisFrame())
        {
            // _rigidbody.AddForce(transform.right * 10000, ForceMode.Force);
            print("Rotate Pressed");
        }
        
        if (movement.x != 0)
        {
            var angleCandidate = Angle + movement.x * Sensetivity * Time.deltaTime;
            if (Mathf.Abs(angleCandidate) < MaxAngle)
            {
                Angle = angleCandidate;
            }
            else
            {
                Angle = MaxAngle * MathF.Sign(angleCandidate);
            }
        }
        else
        {
            if (Angle != 0 && AutoCenter)
            {
                Angle -= MathF.Min(MathF.Abs(Angle), Sensetivity * 2 * Time.deltaTime) * MathF.Sign(Angle);
            }
        }


    }
}
