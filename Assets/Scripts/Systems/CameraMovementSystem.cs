using DefaultNamespace;
using Unity.Entities;
using Unity.Transforms;


[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial class CameraMovementSystem : SystemBase
{
    protected override void OnCreate()
    {
        CheckedStateRef.RequireForUpdate<CameraPoint>();
    }

    protected override void OnUpdate()
    {
        var cam = PlayerCamera.Instance;
        var cameraEntity = SystemAPI.GetSingletonEntity<CameraPoint>();
        var globalTransform = SystemAPI.GetComponent<LocalToWorld>(cameraEntity);
        cam.transform.position = globalTransform.Position;
        cam.transform.rotation = globalTransform.Rotation;
    }
}