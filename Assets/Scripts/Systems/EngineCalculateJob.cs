using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

[BurstCompile]
public partial struct EngineCalculateJob : IJobEntity
{
    private void Execute(ref EngineData engine, in CartInputData input, PhysicsVelocity velocity,
        DynamicBuffer<CurvePoint> curve)
    {
        var currentSpeed = math.length(velocity.Linear);
        if (currentSpeed > engine.MaxSpeed) return;

        var rate = currentSpeed / engine.MaxSpeed;
        engine.CurrentForce = EvaluateCurve(rate, curve) * input.CurrentEngine * engine.MaxForce;
    }

    private float EvaluateCurve(float curveValue, DynamicBuffer<CurvePoint> curve)
    {
        if (curveValue >= curve[^1].Value.x)
            return curve[^1].Value.y;

        if (curveValue <= curve[0].Value.x)
            return curve[0].Value.y;

        var pointLess = float2.zero;
        var pointMore = float2.zero;

        for (var i = 0; i < curve.Length - 1; i++)
        {
            var current = curve[i].Value;
            var next = curve[i + 1].Value;

            if (!(curveValue >= current.x) || !(curveValue <= next.x)) continue;

            pointLess = current;
            pointMore = next;
            break;
        }

        var delta = pointMore - pointLess;
        var t = (curveValue - pointLess.x) / delta.x;
        var y = pointLess.y + t * delta.y;

        return y;
    }
}