using Unity.Entities;
using Unity.Transforms;

namespace DefaultNamespace.Systems
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class CameraMovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            if (!SystemAPI.TryGetSingletonEntity<CameraPoint>(out var cameraEntity))
                return;

            var cam = PlayerCamera.Instance;
            var globalTransform = SystemAPI.GetComponent<LocalToWorld>(cameraEntity);
            cam.transform.position = globalTransform.Position;
            cam.transform.rotation = globalTransform.Rotation;
        }
    }
}