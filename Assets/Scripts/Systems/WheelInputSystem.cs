using System;
using System.Diagnostics;
using mozaAPI;
using RookieCartingClub.Components;
using RookieCartingClub.Components.Replay;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Debug = UnityEngine.Debug;
using static mozaAPI.mozaAPI;


namespace RookieCartingClub.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    public partial class WheelInputSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var mwh = Process.GetCurrentProcess().MainWindowHandle;
            Debug.Log(mwh.ToInt64());
            installMozaSDK();
            var keyboardFilter = new EntityQueryBuilder(Allocator.Temp).WithAll<CartInputData>()
                .WithAll<InputFromWheel, GhostOwnerIsLocal>();
            var playbackFilter = new EntityQueryBuilder(Allocator.Temp).WithNone<ReplayPlayback>();
            CheckedStateRef.RequireForUpdate(CheckedStateRef.GetEntityQuery(keyboardFilter));
            CheckedStateRef.RequireForUpdate(CheckedStateRef.GetEntityQuery(playbackFilter));
        }

        protected override void OnUpdate()
        {
            var code = new ERRORCODE();
            var data = getHIDData(ref code);

            var userControl = new RefRW<CartInputData>();
            var inputSetting = new InputFromWheel();

            foreach (var (_, inputData, keyboardSettings) in SystemAPI
                         .Query<EnabledRefRO<GhostOwnerIsLocal>, RefRW<CartInputData>, RefRO<InputFromWheel>>())
            {
                userControl = inputData;
                inputSetting = keyboardSettings.ValueRO;
            }

            userControl.ValueRW.CurrentEngine = FromMozaToRccAxis(data.throttle);
            userControl.ValueRW.CurrentBreaks = FromMozaToRccAxis(data.brake);
            userControl.ValueRW.CurrentAngle = GetAngleFromMoza(data.steeringWheelAxle, inputSetting);
        }

        private static float FromMozaToRccAxis(short value)
        {
            return (float)(((double)value + short.MaxValue) / ushort.MaxValue);
        }

        private static float GetAngleFromMoza(short value, InputFromWheel inputFromWheel)
        {
            var half = inputFromWheel.WheelDegrees / 2.0f;
            var rotation = 1 - math.abs(value / (double)short.MaxValue);
            rotation *= Math.Sign(value);

            return (float)(-rotation * half * inputFromWheel.SteerMultiplier);
        }
    }
}