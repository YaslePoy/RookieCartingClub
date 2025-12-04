using RookieCartingClub.Authoring;
using RookieCartingClub.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

namespace RookieCartingClub.Systems
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class CameraMovementSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<CameraPoint>();
            RequireForUpdate<CameraData>();
        }

        protected override void OnUpdate()
        {
            var cameraComponent = SystemAPI.GetSingleton<CameraData>();

            if (EntityManager.Exists(cameraComponent.PlayerEntity) == false)
            {
                return;
            }
            
            var cam = PlayerCamera.Instance;
            var cameraPositionEntity = Entity.Null;
            var children = EntityManager.GetBuffer<Child>(cameraComponent.PlayerEntity);
            
            foreach (var child in children)
            {
                if (EntityManager.HasComponent<CameraPoint>(child.Value))
                {
                    cameraPositionEntity = child.Value;
                    break;
                }
            }

            var globalTransform = SystemAPI.GetComponent<LocalToWorld>(cameraPositionEntity);
            cam.transform.position = globalTransform.Position;
            cam.transform.rotation = globalTransform.Rotation;
        }
    }
}