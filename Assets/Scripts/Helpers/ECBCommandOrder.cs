using UnityEngine;

public struct ECBCommandOrder
{
    public static readonly ECBCommandOrder CreateEntity = new(Values.CreateEntity);
    public static readonly ECBCommandOrder Instantiate = new(Values.Instantiate);
    public static readonly ECBCommandOrder AddComponent = new(Values.AddComponent);
    public static readonly ECBCommandOrder AddBuffer = new(Values.AddBuffer);
    public static readonly ECBCommandOrder SetComponentEnabled = new(Values.SetComponentEnabled);
    public static readonly ECBCommandOrder SetComponent = new(Values.SetComponent);
    public static readonly ECBCommandOrder SetBuffer = new(Values.SetBuffer);
    public static readonly ECBCommandOrder AppendToBuffer = new(Values.AppendToBuffer);
    public static readonly ECBCommandOrder RemoveComponent = new(Values.RemoveComponent);
    public static readonly ECBCommandOrder DestroyEntity = new(Values.DestroyEntity);

    private readonly Values Value;

    private ECBCommandOrder(Values value)
    {
        Value = value;
    }

    public static implicit operator int(ECBCommandOrder value)
    {
        return (int)value.Value;
    }

    private enum Values
    {
        CreateEntity = 0,
        Instantiate = 1,
        AddComponent = 2,
        AddBuffer = 3,
        SetComponentEnabled = 4,
        SetComponent = 5,
        SetBuffer = 6,
        AppendToBuffer = 7,
        RemoveComponent = 8,
        DestroyEntity = 9
    }
}
