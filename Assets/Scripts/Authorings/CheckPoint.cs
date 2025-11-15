using Unity.Entities;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public Mesh Mesh;

    public int Index;

    public class CheckPointBaker : Baker<CheckPoint>
    {
        public override void Bake(CheckPoint authoring)
        {

            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new CheckPointData { Index = authoring.Index });
        }
    }
}