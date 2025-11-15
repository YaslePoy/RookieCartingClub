using RookieCartingClub.Components;
using Unity.Entities;
using UnityEngine;

namespace RookieCartingClub.Authoring
{
    public class CameraControlAuthoring : MonoBehaviour
    {
        private class CameraControlAuthoringBaker : Baker<CameraControlAuthoring>
        {
            public override void Bake(CameraControlAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<CameraPoint>(entity);
            }
        }
    }
}