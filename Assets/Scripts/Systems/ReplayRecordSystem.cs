using System;
using System.IO;
using System.Runtime.InteropServices;
using RookieCartingClub.Components;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace RookieCartingClub.Systems
{
    [UpdateInGroup(typeof(CartPhysicsSimulationGroup))]
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

            var stopRequest = SystemAPI.HasSingleton<StopRecord>();
            if (stopRequest)
            {
                WriteCurrentReplay(buffer);

                state.EntityManager.DestroyEntity(SystemAPI.GetSingletonEntity<StopRecord>());
                state.EntityManager.DestroyEntity(SystemAPI.GetSingletonEntity<RecordInput>());
                return;
            }

            buffer.Add(new InputRecord { Input = SystemAPI.GetSingleton<CartInputData>() });
        }

        private static void WriteCurrentReplay(DynamicBuffer<InputRecord> buffer)
        {
            var sizeOf = Marshal.SizeOf(typeof(InputRecord));

            var recorded = buffer.AsNativeArray();
            var length = recorded.Length;
            var finalOutput = new Span<byte>(new byte[length * sizeOf]);

            for (var i = 0; i < buffer.Length; i++)
            {
                var input = recorded[i].Input;
                MemoryMarshal.Write(finalOutput[(sizeOf * i)..], ref input);
            }

            var file = new FileStream(DateTime.Now.ToString("yyyy_MM_dd HH_mm_ss") + ".rir", FileMode.CreateNew);
            file.Write(finalOutput);
            file.Close();
            Debug.Log("Replay saved!");
        }
    }
}