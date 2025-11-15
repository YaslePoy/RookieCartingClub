using Unity.Entities;
using UnityEngine;

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