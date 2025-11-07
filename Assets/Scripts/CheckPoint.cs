using Unity.Entities;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public Collider Collider;

    public int Index;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Collider = gameObject.GetComponent<Collider>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Cart collider"))
            return;

        var parent = other.transform.parent.gameObject;

        parent.GetComponent<CartHandle>().PushCheckPoint(this);
        //print($"Colliding {Index}");
    }
}

public class CheckPointBaker : Baker<CheckPoint>
{
    public override void Bake(CheckPoint authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new CheckPointData { Index = authoring.Index });
    }
}

public struct CheckPointData : IComponentData
{
    public int Index;
}