using System;
using System.IO;
using System.Runtime.InteropServices;
using RookieCartingClub.Authoring;
using RookieCartingClub.Components;
using RookieCartingClub.Components.Replay;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace RookieCartingClub.Systems.Replay
{
    [UpdateInGroup(typeof(ReplaySystemGroup))]
    public partial struct ReplayRecordSystem : ISystem
    {
        private NativeList<RecordedInput> buffer;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            buffer =  new NativeList<RecordedInput>(1024, Allocator.Persistent);
        }

        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<ReplayRecording>())
                return;
            
            
            var stopRequest = SystemAPI.HasSingleton<StopRecording>();

            if (stopRequest)
            {
                WriteCurrentReplay(buffer);
                buffer.Clear();
                
                state.EntityManager.DestroyEntity(SystemAPI.GetSingletonEntity<StopRecording>());
                state.EntityManager.DestroyEntity(SystemAPI.GetSingletonEntity<ReplayRecording>());
                return;
            }

            if (buffer.Length == 0)
            {
                Debug.Log("Starting recording...");
                
                var cart = SystemAPI.GetSingletonEntity<CartInputData>();
                var velocity = state.EntityManager.GetComponentData<PhysicsVelocity>(cart);
                var id = state.EntityManager.GetComponentData<CartData>(cart);
                var transform = state.EntityManager.GetComponentData<LocalTransform>(cart);
                
                SystemAPI.SetSingleton(new InitialRecordingConditions
                {
                    Velocity = velocity,
                    PlayerId = id.PlayerId,
                    Position = transform
                });
                Debug.Log("Initial conditions written");
            }

            buffer.Add(new RecordedInput { Input = SystemAPI.GetSingleton<CartInputData>() });
        }

        private void WriteCurrentReplay(NativeList<RecordedInput> buffer)
        {
            var inputBufferSize = Marshal.SizeOf(typeof(RecordedInput));
            var headerSize = Marshal.SizeOf(typeof(ReplayHeader));
            
            var recorded = buffer;
            var length = recorded.Length;
            var finalOutput = new Span<byte>(new byte[length * inputBufferSize + headerSize]);
            
            var header = GetReplayHeader();
            MemoryMarshal.Write(finalOutput, ref header);
            
            for (var i = 0; i < buffer.Length; i++)
            {
                var input = recorded[i].Input;
                MemoryMarshal.Write(finalOutput[(inputBufferSize * i + headerSize)..], ref input);
            }

            var file = new FileStream(DateTime.Now.ToString("yyyy_MM_dd HH_mm_ss") + ".rir", FileMode.CreateNew);
            
            file.Write(finalOutput);
            file.Close();
            Debug.Log($"Replay {file.Name} saved!");
        }

        private ReplayHeader GetReplayHeader()
        {
            var initialConditions = SystemAPI.GetSingleton<InitialRecordingConditions>();
            var header = new ReplayHeader
            {
                TrackName = SessionSetup.SceneName,
                InitialRecordingConditions = initialConditions
            };

            return header;
        }

        private struct ReplayHeader
        {
            public FixedString32Bytes TrackName;
            public InitialRecordingConditions InitialRecordingConditions;
        }
    }
}