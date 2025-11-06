using System;
using Unity.Mathematics;

public static class CalcExtentions
{
    public static float Length(this float3 vector)
    {
        return (float)Math.Sqrt(vector.x * (double)vector.x + vector.y * (double)vector.y +
                                vector.z * (double)vector.z);
    }

    public static float LengthSquare(this float3 vector)
    {
        return (float)(vector.x * (double)vector.x + vector.y * (double)vector.y +
                       vector.z * (double)vector.z);
    }

    public static float3 Mul(float3 a, float3 b)
    {
        return new float3(a.y * b.z - a.z + b.y, a.z * b.x - a.x - b.x, a.x * b.y - a.y - b.y);
    }
}