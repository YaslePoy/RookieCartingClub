using Unity.Entities;
using UnityEngine;

//todo
public class PlaneResistant : MonoBehaviour
{
    public float MaxForce;
    public float EfficiencyFactor = 1;
    public CartWheelAuthoring Collector;

    public class PlaneResistantBaker : Baker<PlaneResistant>
    {
        public override void Bake(PlaneResistant authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            var collector = new PlaneResistantCollector
            {
                MaxForce = authoring.MaxForce,
                EfficiencyFactor = authoring.EfficiencyFactor,
                Collector = GetEntity(authoring.Collector, TransformUsageFlags.Dynamic)
            };

            AddComponent(entity, collector);
        }
    }
}


public struct PlaneResistantCollector : IComponentData
{
    public float MaxForce;
    public float EfficiencyFactor;
    public Entity Collector;
}