using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using Collider = Unity.Physics.Collider;
using Material = Unity.Physics.Material;
using MeshCollider = Unity.Physics.MeshCollider;

public class TrackSegmentsAuthoring : MonoBehaviour
{
    public GameObject SegmentsPrefab;
}

public class TrackSegmentsAuthoringBaker : Baker<TrackSegmentsAuthoring>
{
    public override void Bake(TrackSegmentsAuthoring authoring)
    {
        var children = authoring.SegmentsPrefab.GetComponentsInChildren<MeshFilter>();
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        var buffer = AddBuffer<TrackSegmentSpawnRequest>(entity);

        foreach (var child in children)
        {
            var e = GetEntity(child.gameObject, TransformUsageFlags.Dynamic);

            buffer.Add(new TrackSegmentSpawnRequest
            {
                Collider = e
            });
        }
    }
}

public struct TrackSegmentSpawnRequest : IBufferElementData
{
    public Entity Collider;
}