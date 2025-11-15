using Unity.Entities;
using UnityEngine;

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