using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Windows;

public class ReplayAuthoring : MonoBehaviour
{
    public string ReplayName;

    private class ReplayAuthoringBaker : Baker<ReplayAuthoring>
    {
        public override unsafe void Bake(ReplayAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var buffer = AddBuffer<InputRecord>(entity);
            AddComponent<ReplayInput>(entity);

            byte[] fileData = Array.Empty<byte>();
            try
            {
                fileData = File.ReadAllBytes(authoring.ReplayName);
            }
            finally
            {
                fixed (byte* raw = fileData)
                {
                    var converted = new Span<InputRecord>((InputRecord*)raw, fileData.Length / sizeof(InputRecord));
                    var na = new NativeArray<InputRecord>(converted.ToArray(), Allocator.Temp);
                    buffer.AddRange(na);
                    na.Dispose();
                }
            }
        }
    }
}