using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public class CameraControlAuthoring : MonoBehaviour
{
    private class CameraControlAuthoringBaker : Baker<CameraControlAuthoring>
    {
        public override void Bake(CameraControlAuthoring authoring)
        {
            var entity = GetEntity();
            AddComponent<CameraPoint>(entity);
        }
    }
}

public struct CameraPoint: IComponentData
{
}