using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class UserControl : NetworkBehaviour
{
    public Transform CameraPosition;
    private InputAction _forceAction;
    private InputAction _rotateAction;
    private Rigidbody _rigidbody;
    public bool AutoCenter = true;
    
    public NetworkVariable<float> Angle = new (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public float MaxAngle;
    public float Sensetivity;

    public NetworkVariable<float> Engine = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<float> Breaks = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _forceAction = InputSystem.actions.FindAction("Move");
        _rotateAction = InputSystem.actions.FindAction("Jump");
        _rigidbody = GetComponent<Rigidbody>();

        if (IsClient && IsOwner)
        {
            var cam = GameObject.Find("Camera");
            cam.transform.parent = this.transform;
            cam.transform.localPosition = CameraPosition.localPosition;
            cam.transform.localRotation = CameraPosition.localRotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        
        if (!AutoCenter)
            return;
        var movement = _forceAction.ReadValue<Vector2>();
        
        Engine.Value = 0;
        Breaks.Value = 0;
        
        if ( movement.y != 0)
        {
            Engine.Value = 0;
            Breaks.Value = 0;
            if (movement.y > 0)
            {
                Engine.Value = movement.y;
            }
            else
            {
                Breaks.Value = -movement.y;
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
            var delta = movement.x * Sensetivity * Time.deltaTime;
            if (MathF.Sign(delta) != MathF.Sign(Angle.Value))
            {
                delta *= 3.5f;
            }
            var angleCandidate = Angle.Value + delta;
            if (Mathf.Abs(angleCandidate) < MaxAngle)
            {
                Angle.Value = angleCandidate;
            }
            else
            {
                Angle.Value = MaxAngle * MathF.Sign(angleCandidate);
            }
        }
        else
        {
            if (Angle.Value != 0 && AutoCenter)
            {
                Angle.Value -= MathF.Min(MathF.Abs(Angle.Value), Sensetivity * 2 * Time.deltaTime) * MathF.Sign(Angle.Value);
            }
        }


    }
}
