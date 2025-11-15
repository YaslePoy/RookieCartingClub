using Unity.Entities;

public struct PlaneResistantCollector : IComponentData
{
    public float MaxForce;
    public float EfficiencyFactor;
    public Entity Collector;
}