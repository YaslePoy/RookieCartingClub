using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ControlRotating : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private UserControl _userControl;

    private void Start()
    {
        _userControl = gameObject.GetComponentInParent<UserControl>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.localRotation = Quaternion.AngleAxis(_userControl.CurrentAngle, transform.up);
    }
}


public class ForwardWheelBaker : Baker<ControlRotating>
{
    public override void Bake(ControlRotating authoring)
    {
        var e = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent<FrontWheel>(e);
    }
}


public struct FrontWheel : IComponentData
{
}

public struct RearWheel : IComponentData
{
}

public struct ForceApplyRequest : IBufferElementData
{
    public float3 Force;
}