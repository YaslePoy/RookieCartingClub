using Unity.Entities;
using UnityEngine;

public class BreaksHandle : MonoBehaviour
{
    public PlaneResistant BreakResistant;
    private UserControl _control;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _control = GetComponentInParent<UserControl>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        BreakResistant.K = _control.CurrentBreaks;
    }
}

public class RearWheelBaker : Baker<BreaksHandle>
{
    public override void Bake(BreaksHandle authoring)
    {
        var e = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent<CartWheel>(e);
        AddComponent<RearWheel>(e);
    }
}