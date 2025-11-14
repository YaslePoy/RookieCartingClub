using System;
using System.IO;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Windows;

[UpdateInGroup(typeof(CartPhysicsSimulationGroup))]
// [UpdateBefore(typeof(WheelRotatingSystem))]
public partial struct ReplayRecordSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<RecordInput>();
        state.RequireForUpdate<CartInputData>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var buffer = SystemAPI.GetSingletonBuffer<InputRecord>();

        if (SystemAPI.HasSingleton<StopRecord>())
        {
            var sizeOf = Marshal.SizeOf(typeof(InputRecord));

            using var recorded = buffer.ToNativeArray(Allocator.Temp);
            var length = recorded.Length;
            var finalOutput = new Span<byte>(new byte[length * sizeOf]);
            for (int i = 0; i < buffer.Length; i++)
            {
                var input = recorded[i].Input;
                MemoryMarshal.Write(finalOutput[(sizeOf * i)..], ref input);
            }

            var file = new FileStream(DateTime.Now.ToString("yyyy_MM_dd HH_mm_ss") + ".rir", FileMode.CreateNew);
            file.Write(finalOutput);
            file.Close();
            Debug.Log("Replay saved!");
            state.EntityManager.DestroyEntity(SystemAPI.GetSingletonEntity<StopRecord>());
            state.EntityManager.DestroyEntity(SystemAPI.GetSingletonEntity<RecordInput>());

            return;
        }

        buffer.Add(new InputRecord { Input = SystemAPI.GetSingleton<CartInputData>() });
    }

    public void OnDestroy(ref SystemState state)
    {
    }
}