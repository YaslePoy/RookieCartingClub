using System;
using System.Runtime.InteropServices;
using RookieCartingClub.Systems.Replay;
using Unity.Collections;

namespace RookieCartingClub.Components.Replay
{
    public struct Replay
    {
        public NativeArray<InitialRecordingConditions> InitialRecordingConditions;
        public NativeArray<NativeList<RecordedInput>> Inputs;

        public static Replay Decode(byte[] data)
        {
            var replay = new Replay
            {
                Inputs = new NativeArray<NativeList<RecordedInput>>(1, Allocator.Persistent),
                InitialRecordingConditions = new NativeArray<InitialRecordingConditions>(1, Allocator.Persistent)
            };

            var rawReplay = data.AsSpan();
            
            var header = MemoryMarshal.Read<ReplayRecordSystem.ReplayHeader>(rawReplay);
            replay.InitialRecordingConditions[0] = header.InitialRecordingConditions;

            var inputs = MemoryMarshal.Cast<byte, RecordedInput>(rawReplay[ReplayRecordSystem.ReplayHeader.Size..]);
            var recordedInputs = new NativeList<RecordedInput>(inputs.Length, Allocator.Persistent);
            recordedInputs.AddRange(new NativeArray<RecordedInput>(inputs.ToArray(), Allocator.Temp));
            replay.Inputs[0] = recordedInputs;
            return replay;
        }
    }
}