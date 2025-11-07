using System;
using Unity.Entities;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardInput : NetworkBehaviour
{
    public float MaxAngle;
    public float Sensetivity;
    private InputAction _forceAction;
    private InputAction _rotateAction;

    private UserControl _userControl;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (!IsOwner) return;
        _userControl = GetComponent<UserControl>();
        _forceAction = InputSystem.actions.FindAction("Move");
        _rotateAction = InputSystem.actions.FindAction("Jump");
    }

    // Update is called once per frame
    private void Update()
    {
        if (!IsOwner) return;

        var movement = _forceAction.ReadValue<Vector2>();

        _userControl.Engine.Value = 0;
        _userControl.Breaks.Value = 0;

        if (movement.y != 0)
        {
            _userControl.Engine.Value = 0;
            _userControl.Breaks.Value = 0;
            if (movement.y > 0)
                _userControl.Engine.Value = movement.y;
            else
                _userControl.Breaks.Value = -movement.y;
            // _rigidbody.AddForce(transform.forward * (movement.y * 100));
        }

        if (_rotateAction.WasPressedThisFrame())
            // _rigidbody.AddForce(transform.right * 10000, ForceMode.Force);
            print("Rotate Pressed");

        if (movement.x != 0)
        {
            var delta = movement.x * Sensetivity * Time.deltaTime;
            if (MathF.Sign(delta) != MathF.Sign(_userControl.Angle.Value)) delta *= 3.5f;
            var angleCandidate = _userControl.Angle.Value + delta;
            if (Mathf.Abs(angleCandidate) < MaxAngle)
                _userControl.Angle.Value = angleCandidate;
            else
                _userControl.Angle.Value = MaxAngle * MathF.Sign(angleCandidate);
        }
        else
        {
            if (_userControl.Angle.Value != 0)
                _userControl.Angle.Value -=
                    MathF.Min(MathF.Abs(_userControl.Angle.Value), Sensetivity * 2 * Time.deltaTime) *
                    MathF.Sign(_userControl.Angle.Value);
        }
    }
}

public class KeyboardInputBaker : Baker<KeyboardInput>
{
    public override void Bake(KeyboardInput authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new InputFromKeyboard
        {
            MaxAngle = authoring.MaxAngle,
            Sensetivity = authoring.Sensetivity
        });
    }
}

public struct InputFromKeyboard : IComponentData
{
    public float MaxAngle;
    public float Sensetivity;
}