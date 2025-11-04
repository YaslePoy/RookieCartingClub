using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class KeyboardInputSystem : SystemBase
{
    private InputAction _forceAction;

    protected override void OnCreate()
    {
        _forceAction = InputSystem.actions.FindAction("Move");
    }

    protected override void OnUpdate()
    {
        var movement = _forceAction.ReadValue<Vector2>();
        var cartEntity = SystemAPI.GetSingletonEntity<CartInputData>();
        var _userControl = SystemAPI.GetComponentRW<CartInputData>(cartEntity);
        var inputSetting = SystemAPI.GetSingleton<InputFromKeyboard>();
        _userControl.ValueRW.CurrentEngine = 0;
        _userControl.ValueRW.CurrentBreaks = 0;
        if (movement.y != 0)
        {
            if (movement.y > 0)
            {
                _userControl.ValueRW.CurrentEngine = MathF.Round(movement.y);
            }
            else
            {
                _userControl.ValueRW.CurrentBreaks = MathF.Round(-movement.y);
            }
        }

        if (movement.x != 0)
        {
            var delta = movement.x * inputSetting.Sensetivity * SystemAPI.Time.DeltaTime;
            if (MathF.Sign(delta) != MathF.Sign(_userControl.ValueRW.CurrentAngle))
            {
                delta *= 3.5f;
            }

            var angleCandidate = _userControl.ValueRW.CurrentAngle + delta;
            if (Mathf.Abs(angleCandidate) < inputSetting.MaxAngle)
            {
                _userControl.ValueRW.CurrentAngle = angleCandidate;
            }
            else
            {
                _userControl.ValueRW.CurrentAngle = inputSetting.MaxAngle * MathF.Sign(angleCandidate);
            }
        }
        else
        {
            if (_userControl.ValueRW.CurrentAngle != 0)
            {
                _userControl.ValueRW.CurrentAngle -=
                    MathF.Min(MathF.Abs(_userControl.ValueRW.CurrentAngle),
                        inputSetting.Sensetivity * 2 * SystemAPI.Time.DeltaTime) *
                    MathF.Sign(_userControl.ValueRW.CurrentAngle);
            }
        }
    }
}