using RookieCartingClub.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace RookieCartingClub.Systems
{
    [BurstCompile]
    [WithAll(typeof(FrontWheel))]
    public partial struct WheelRotatingSystemJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<CartInputData> InputLookup;

        private void Execute(Parent parent, ref LocalTransform transform)
        {
            var input = InputLookup[parent.Value];
            var angle = input.CurrentAngle * math.TORADIANS;
            transform.Rotation = quaternion.AxisAngle(transform.Up(), angle);
        }
    }
}