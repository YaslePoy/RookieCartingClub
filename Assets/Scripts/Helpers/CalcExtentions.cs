using Unity.Transforms;
using UnityEngine;

public static class CalcExtentions
{
    public static LocalToWorld GetLocalToWorld(this Transform transform)
    {
        var ltw = new LocalToWorld();
        ltw.Value.c0.xyz = transform.right;
        ltw.Value.c1.xyz = transform.up;
        ltw.Value.c2.xyz = transform.forward;
        ltw.Value.c3.xyz = transform.position;
        return ltw;
    }
}