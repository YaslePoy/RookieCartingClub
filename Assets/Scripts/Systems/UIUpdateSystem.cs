using RookieCartingClub.Authoring;
using RookieCartingClub.Components;
using RookieCartingClub.Components.Replay;
using RookieCartingClub.Ui;
using Unity.Entities;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace RookieCartingClub.Systems
{
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
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

            (RefRO<LocalVelocity> velocity, RefRO<LocalToWorld> position, RefRO<CartData> data) playerData = (default, default, default);
            
            foreach (var data in SystemAPI
                         .Query<RefRO<LocalVelocity>, RefRO<LocalToWorld>, RefRO<CartData>>()
                         .WithAll<GhostOwnerIsLocal>())
            {
                playerData = data;
            }
            
            
            UI.Instance.Cart = raceState.Racers.Find(i => i.PlayerId == playerData.data.ValueRO.PlayerId);
            UI.Instance.VelocityProvider = new ConstantVelocityProvider { Velocity = playerData.velocity.ValueRO.Velocity };
            MapHandle.Instance.CartPosition = playerData.position.ValueRO.Position;
            MapHandle.Instance.MoveSelf();
            
            if (UI.Instance.Buttons.IsRecordSwitchRequest)
                SwitchRecord(UI.Instance.Recording);
            
            if (UI.Instance.Buttons.InPitRequest)
                SendInPitRequest();


            UI.Instance.UpdateUI();

        }

        private void SwitchRecord(bool instanceRecording)
        {
            if (instanceRecording)
                StartRecording();
            else
                FinishRecording();
        }

        private void FinishRecording()
        {
            SystemAPI.SetSingleton(new ReplayRecording
            {
                State = RecordingState.Stopping
            });        
        }

        private void StartRecording()
        {
            SystemAPI.SetSingleton(new ReplayRecording
            {
                State = RecordingState.Starting
            });
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