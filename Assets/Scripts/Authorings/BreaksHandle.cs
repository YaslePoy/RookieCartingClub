using Unity.Entities;
using UnityEngine;

//todo
public class BreaksHandle : MonoBehaviour
{
}

public class RearWheelBaker : Baker<BreaksHandle>
{
    public override void Bake(BreaksHandle authoring)
    {
        var e = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent<RearWheel>(e);
    }
}