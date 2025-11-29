using RookieCartingClub.Authoring;
using RookieCartingClub.Components;
using RookieCartingClub.Ui;
using Unity.Entities;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace RookieCartingClub.Systems
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class UIUpdateSystem : SystemBase
    {
        protected override void OnCreate()
        {
            CheckedStateRef.RequireForUpdate<CartData>();
        }

        protected override void OnUpdate()
        {
            var raceStateEntity = SystemAPI.GetSingletonEntity<TrackPositionsCollection>(); //rc is Race Control
            var raceState = CheckedStateRef.EntityManager.GetComponentObject<RaceState>(raceStateEntity);


            foreach (var (velocityRO, positionRo, cartData) in SystemAPI
                         .Query<RefRO<LocalVelocity>, RefRO<LocalToWorld>, RefRO<CartData>>()
                         .WithAll<GhostOwnerIsLocal>())
            {
                if (UI.Instance.Buttons.IsRecordSwitchRequest)
                    StartRecord();
                
                UI.Instance.VelocityProvider = new ConstantVelocityProvider { Velocity = velocityRO.ValueRO.Velocity };

                if (UI.Instance.Buttons.InPitRequest)
                    SendInPitRequest();

                UI.Instance.Cart = raceState.Racers.Find(i => i.PlayerId == cartData.ValueRO.PlayerId);

                UI.Instance.UpdateUI();
                MapHandle.Instance.CartPosition = positionRo.ValueRO.Position;
                MapHandle.Instance.MoveSelf();
            }
        }

        private void StartRecord()
        {
            
        }

        private void SendInPitRequest()
        {
            var id = UI.Instance.Cart.PlayerId;

            var replaceEntity = Entity.Null;

            foreach (var (data, entity) in SystemAPI.Query<RefRO<CartData>>().WithEntityAccess())
                if (data.ValueRO.PlayerId == id)
                {
                    replaceEntity = entity;
                    break;
                }

            EntityManager.SetComponentEnabled<TrackPlacementRequest>(replaceEntity, true);
            EntityManager.SetComponentData(replaceEntity, new TrackPlacementRequest { CollectionId = 1 });
        }
    }

    public class ConstantVelocityProvider : IVelocityProvider
    {
        public Vector3 Velocity { get; set; }
    }

    public interface IVelocityProvider
    {
        Vector3 Velocity { get; }
    }
}