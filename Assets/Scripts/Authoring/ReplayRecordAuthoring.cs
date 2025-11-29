using RookieCartingClub.Components;
using RookieCartingClub.Components.Replay;
using Unity.Entities;
using UnityEngine;

namespace RookieCartingClub.Authoring
{
    public class ReplayRecordAuthoring : MonoBehaviour
    {
        private class ReplayRecordAuthoringBaker : Baker<ReplayRecordAuthoring>
        {
            public override void Bake(ReplayRecordAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddBuffer<RecordedInput>(entity);
                AddComponent<ReplayRecording>(entity);
            }
        }
    }
}