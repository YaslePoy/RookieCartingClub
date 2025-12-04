using RookieCartingClub.Authoring;
using RookieCartingClub.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

namespace RookieCartingClub.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class CameraMovementSystem : SystemBase
    {

        protected override void OnCreate()
        {

            CheckedStateRef.RequireForUpdate<CameraPoint>();
            var query = new EntityQueryBuilder(Allocator.Temp).WithAll<CartData, GhostOwnerIsLocal>();
            CheckedStateRef.RequireAnyForUpdate(CheckedStateRef.GetEntityQuery(query));
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
            var cameraEntity = Entity.Null;
            var children = EntityManager.GetBuffer<Child>(cameraComponent.PlayerEntity);
            
            foreach (var child in children)
            {
                
                if (EntityManager.HasComponent<CameraPoint>(child.Value))
                {
                    cameraEntity = child.Value;
                    break;
                }
            }

            var globalTransform = SystemAPI.GetComponent<LocalToWorld>(cameraEntity);
            cam.transform.position = globalTransform.Position;
            cam.transform.rotation = globalTransform.Rotation;
        }
    }
}