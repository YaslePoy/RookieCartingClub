using System;
using System.IO;
using System.Runtime.InteropServices;
using RookieCartingClub.Authoring;
using RookieCartingClub.Components;
using RookieCartingClub.Components.Replay;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace RookieCartingClub.Systems.Replay
{
    [UpdateInGroup(typeof(ReplaySystemGroup))]
    public partial struct ReplayRecordSystem : ISystem
    {
        public static double LastTime;

        private NativeList<RecordedInput> buffer;
        private EntityQuery playerQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ReplayRecording>();
            buffer = new NativeList<RecordedInput>(1024, Allocator.Persistent);
            state.EntityManager.CreateSingleton<ReplayRecording>();
            var builder = new EntityQueryBuilder(Allocator.Temp).WithAll<CartInputData>().WithNone<Prefab>();
            playerQuery = state.GetEntityQuery(builder);
        }

        public void OnUpdate(ref SystemState state)
        {
            var replayState = SystemAPI.GetSingleton<ReplayRecording>();

            switch (replayState.State)
            {
                case RecordingState.None:
                    return;

                case RecordingState.Starting:
                {
                    replayState.State = RecordingState.Recording;
                    Debug.Log("Starting recording...");

                    var cart = playerQuery.GetSingletonEntity();
                    var velocity = state.EntityManager.GetComponentData<PhysicsVelocity>(cart);
                    var id = state.EntityManager.GetComponentData<CartData>(cart);
                    var transform = state.EntityManager.GetComponentData<LocalTransform>(cart);

                    if (SystemAPI.HasSingleton<InitialRecordingConditions>() == false)
                    {
                        state.EntityManager.CreateSingleton<InitialRecordingConditions>();
                    }

                    SystemAPI.SetSingleton(new InitialRecordingConditions
                    {
                        Velocity = velocity,
                        PlayerId = id.PlayerId,
                        Position = transform
                    });


                    Debug.Log("Initial conditions written");
                    break;
                }
                case RecordingState.Stopping:
                    replayState.State = RecordingState.None;
                    WriteCurrentReplay(buffer);
                    buffer.Clear();

                    break;
                case RecordingState.Recording:
                    buffer.Add(new RecordedInput { Input = SystemAPI.GetSingleton<CartInputData>() });
                    var now = SystemAPI.Time.ElapsedTime;

                    Debug.Log($"Tick rate: {1f / (now - LastTime)}");
                    LastTime = now;
                    break;
            }

            SystemAPI.SetSingleton(replayState);
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
            var headerx = MemoryMarshal.Read<ReplayHeader>(finalOutput);
            for (var i = 0; i < buffer.Length; i++)
            {
                var input = recorded[i];
                MemoryMarshal.Write(finalOutput[(inputBufferSize * i + headerSize)..], ref input);
            }

            header = MemoryMarshal.Read<ReplayHeader>(finalOutput);

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

        public struct ReplayHeader
        {
            public FixedString32Bytes TrackName;
            public InitialRecordingConditions InitialRecordingConditions;
            public static readonly int Size = Marshal.SizeOf<ReplayHeader>();
        }
    }
}