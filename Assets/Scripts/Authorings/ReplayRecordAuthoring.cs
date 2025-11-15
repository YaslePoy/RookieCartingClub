using Unity.Entities;
using UnityEngine;


public class ReplayRecordAuthoring : MonoBehaviour
{
    private class ReplayRecordAuthoringBaker : Baker<ReplayRecordAuthoring>
    {
        public override void Bake(ReplayRecordAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddBuffer<InputRecord>(entity);
            AddComponent<RecordInput>(entity);
        }
    }
}