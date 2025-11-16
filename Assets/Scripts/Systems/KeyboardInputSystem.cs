using System;
using RookieCartingClub.Components;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RookieCartingClub.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation|WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    public partial class KeyboardInputSystem : SystemBase
    {
        private InputAction _forceAction;
        private InputAction _stopRecordAction;

        protected override void OnCreate()
        {
            _forceAction = InputSystem.actions.FindAction("Move");
            _stopRecordAction = InputSystem.actions.FindAction("Crouch");
            CheckedStateRef.RequireForUpdate<CartInputData>();
            CheckedStateRef.RequireForUpdate<InputFromKeyboard>();
        }

        protected override void OnUpdate()
        {
            var isStopingRecording = _stopRecordAction.WasPressedThisFrame();
            if (isStopingRecording) EntityManager.CreateEntity(typeof(StopRecord));

            var isRequireReplay = SystemAPI.HasSingleton<ReplayInput>();
            if (isRequireReplay) return;

            var cartEntity = SystemAPI.GetSingletonEntity<CartInputData>();
            var userControl = SystemAPI.GetComponentRW<CartInputData>(cartEntity);
            var inputSetting = SystemAPI.GetSingleton<InputFromKeyboard>();
            userControl.ValueRW.CurrentEngine = 0;
            userControl.ValueRW.CurrentBreaks = 0;

            var movement = _forceAction.ReadValue<Vector2>();

            var isAcceleratingOrBreaking = movement.y != 0;
            if (isAcceleratingOrBreaking) SetupEngineAndBreaks(movement, userControl);

            var isSteering = movement.x != 0;
            if (isSteering)
                SetupWheelAngleWithSteering(movement, inputSetting, userControl, SystemAPI.Time.DeltaTime);
            else
                ReturnWheelToCenter(userControl, inputSetting, SystemAPI.Time.DeltaTime);
        }

        private static void ReturnWheelToCenter(RefRW<CartInputData> userControl, InputFromKeyboard inputSetting,
            float deltaTime)
        {
            if (userControl.ValueRW.CurrentAngle != 0)
                userControl.ValueRW.CurrentAngle -=
                    MathF.Min(MathF.Abs(userControl.ValueRW.CurrentAngle),
                        inputSetting.Sensetivity * 2 * deltaTime) *
                    MathF.Sign(userControl.ValueRW.CurrentAngle);
        }

        private static void SetupEngineAndBreaks(Vector2 movement, RefRW<CartInputData> userControl)
        {
            if (movement.y > 0)
                userControl.ValueRW.CurrentEngine = MathF.Round(movement.y);
            else
                userControl.ValueRW.CurrentBreaks = MathF.Round(-movement.y);
        }

        private static void SetupWheelAngleWithSteering(Vector2 movement, InputFromKeyboard inputSetting,
            RefRW<CartInputData> userControl, float deltaTime)
        {
            const float acceleratedSteering = 3.5f;
            var angleDelta = movement.x * inputSetting.Sensetivity * deltaTime;
            if (MathF.Sign(angleDelta) != MathF.Sign(userControl.ValueRW.CurrentAngle)) angleDelta *= acceleratedSteering;

            var angleCandidate = userControl.ValueRW.CurrentAngle + angleDelta;

            if (Mathf.Abs(angleCandidate) < inputSetting.MaxAngle)
                userControl.ValueRW.CurrentAngle = angleCandidate;
            else
                userControl.ValueRW.CurrentAngle = inputSetting.MaxAngle * MathF.Sign(angleCandidate);
        }
    }

    public struct StopRecord : IComponentData
    {
    }
}