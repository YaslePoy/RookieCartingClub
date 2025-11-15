using Unity.Entities;

public struct CartWheel : IComponentData
{
    public float MaxResistance;
    public float ForcePart;
    public float Mass;
    public float Friction;
}