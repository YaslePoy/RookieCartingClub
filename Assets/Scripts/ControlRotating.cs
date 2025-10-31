using Unity.Entities;
using UnityEngine;

public class ControlRotating : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private UserControl _userControl;
    void Start()
    {
       _userControl = gameObject.GetComponentInParent<UserControl>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localRotation = Quaternion.AngleAxis(_userControl.CurrentAngle, transform.up);
    }
}


public class ForwardWheelBaker : Baker<ControlRotating>
{
    public override void Bake(ControlRotating authoring)
    {
        var e = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent<CartWheel>(e);
        AddComponent<FrontWheel>(e);
    }
}

public struct CartWheel : IComponentData
{
    
}

public struct FrontWheel : IComponentData
{
}

public struct RearWheel : IComponentData
{
}