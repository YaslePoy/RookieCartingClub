using Unity.Entities;
using Unity.Physics.Authoring;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public Mesh Mesh;

    public int Index;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

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
        
        // var basicGO = new GameObject();
        //
        // var colider = basicGO.AddComponent<MeshCollider>();
        // colider.sharedMesh = authoring.Mesh;
        // colider.convex = true;
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        //
        AddComponent(entity, new CheckPointData { Index = authoring.Index });
    }
}

public struct CheckPointData : IComponentData
{
    public int Index;
}