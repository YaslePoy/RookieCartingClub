using RookieCartingClub.Authoring;
using RookieCartingClub.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

namespace RookieCartingClub.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation|WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class CameraMovementSystem : SystemBase
    {
        private ComponentLookup<CameraPoint>  _cameraLookup;
        protected override void OnCreate()
        {
            _cameraLookup = CheckedStateRef.GetComponentLookup<CameraPoint>(true);
            
            CheckedStateRef.RequireForUpdate<CameraPoint>();
            var query = new EntityQueryBuilder(Allocator.Temp).WithAll<CartData>().WithAll<GhostOwnerIsLocal>();
            CheckedStateRef.RequireAnyForUpdate(CheckedStateRef.GetEntityQuery(query));
        }

        protected override void OnUpdate()
        {
            _cameraLookup.Update(ref CheckedStateRef);
            var cam = PlayerCamera.Instance;

            var cameraEntity = Entity.Null;
            foreach (var (_, _, children) in SystemAPI.Query<EnabledRefRO<GhostOwnerIsLocal>, RefRO<CartData>, DynamicBuffer<Child>>())
            {
                foreach (var child in children)
                {
                    if (_cameraLookup.HasComponent(child.Value))
                    {
                        cameraEntity = child.Value;
                        break;
                    }
                }
            }
            
            var globalTransform = SystemAPI.GetComponent<LocalToWorld>(cameraEntity);
            cam.transform.position = globalTransform.Position;
            cam.transform.rotation = globalTransform.Rotation;
        }
    }
}