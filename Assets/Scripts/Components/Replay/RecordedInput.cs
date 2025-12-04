using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace RookieCartingClub.Components.Replay
{
    public struct RecordedInput : IBufferElementData
    {
        public CartInputData Input;
        
        public static readonly int Size = Marshal.SizeOf<RecordedInput>();
    }

    public struct InitialRecordingConditions : IComponentData
    {
        public LocalTransform Position;
        public PhysicsVelocity Velocity;
        public int PlayerId;
    }
}