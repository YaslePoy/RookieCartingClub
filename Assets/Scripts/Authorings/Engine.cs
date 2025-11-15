using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class Engine : MonoBehaviour
{
    public AnimationCurve AnimationCurve;
    public float MaxForce;

    public float MaxSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private UserControl _control;
    protected float _currentForce;
    private Rigidbody _rigidbody;
    public float CurrentForce => _currentForce;


    public class EngineBaker : Baker<Engine>
    {
        public override void Bake(Engine authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var buffer = AddBuffer<CurvePoint>(entity);
            foreach (var keyframe in authoring.AnimationCurve.keys)
                buffer.Add(new CurvePoint { Value = new float2(keyframe.time, keyframe.value) });

            AddComponent(entity, new EngineData
            {
                MaxForce = authoring.MaxForce,
                MaxSpeed = authoring.MaxSpeed
            });
        }
    }
}


public struct EngineData : IComponentData
{
    public float MaxForce;
    public float MaxSpeed;
    public float CurrentForce;
}


public struct CurvePoint : IBufferElementData
{
    public float2 Value;
}