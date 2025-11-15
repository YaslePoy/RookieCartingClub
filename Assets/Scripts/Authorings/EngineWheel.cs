using Unity.Entities;
using UnityEngine;

//todo
public class EngineWheel : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float Part;
    public PlaneResistant EngineResistant;

    public class EngineWheelBaker : Baker<EngineWheel>
    {
        public override void Bake(EngineWheel authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EngineWheelData
            {
                Part = authoring.Part,
                EngineResistant = GetEntity(authoring.EngineResistant, TransformUsageFlags.Dynamic)
            });
        }
    }
}


public struct EngineWheelData : IComponentData
{
    public float Part;
    public Entity EngineResistant;
}