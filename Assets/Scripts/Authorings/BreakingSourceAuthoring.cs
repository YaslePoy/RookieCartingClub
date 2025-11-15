using Unity.Entities;
using UnityEngine;

public class BreakingSourceAuthoring : MonoBehaviour
{
    public UserControl Source;

    private class BreakingSourceAuthoringBaker : Baker<BreakingSourceAuthoring>
    {
        public override void Bake(BreakingSourceAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BreakingSource
            {
                Source = GetEntity(authoring.Source, TransformUsageFlags.Dynamic)
            });
        }
    }
}