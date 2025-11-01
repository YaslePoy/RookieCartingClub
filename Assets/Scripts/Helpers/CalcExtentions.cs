
using System;
using Unity.Mathematics;

public static class CalcExtentions
{
    public static float Length(this float3 vector)
    {
        return (float) Math.Sqrt((double) vector.x * (double) vector.x + (double) vector.y * (double) vector.y + (double) vector.z * (double) vector.z);
    }
}