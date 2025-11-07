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
        if (!SystemAPI.TryGetSingletonEntity<CartInputData>(out var cartEntity)) return;

        var movement = _forceAction.ReadValue<Vector2>();

        var userControl = SystemAPI.GetComponentRW<CartInputData>(cartEntity);
        var inputSetting = SystemAPI.GetSingleton<InputFromKeyboard>();
        userControl.ValueRW.CurrentEngine = 0;
        userControl.ValueRW.CurrentBreaks = 0;
        if (movement.y != 0)
        {
            if (movement.y > 0)
                userControl.ValueRW.CurrentEngine = MathF.Round(movement.y);
            else
                userControl.ValueRW.CurrentBreaks = MathF.Round(-movement.y);
        }

        if (movement.x != 0)
        {
            var delta = movement.x * inputSetting.Sensetivity * SystemAPI.Time.DeltaTime;
            if (MathF.Sign(delta) != MathF.Sign(userControl.ValueRW.CurrentAngle)) delta *= 3.5f;

            var angleCandidate = userControl.ValueRW.CurrentAngle + delta;
            if (Mathf.Abs(angleCandidate) < inputSetting.MaxAngle)
                userControl.ValueRW.CurrentAngle = angleCandidate;
            else
                userControl.ValueRW.CurrentAngle = inputSetting.MaxAngle * MathF.Sign(angleCandidate);
        }
        else
        {
            if (userControl.ValueRW.CurrentAngle != 0)
                userControl.ValueRW.CurrentAngle -=
                    MathF.Min(MathF.Abs(userControl.ValueRW.CurrentAngle),
                        inputSetting.Sensetivity * 2 * SystemAPI.Time.DeltaTime) *
                    MathF.Sign(userControl.ValueRW.CurrentAngle);
        }
    }
}