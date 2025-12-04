using RookieCartingClub.Authoring;
using RookieCartingClub.Components;
using RookieCartingClub.Components.Replay;
using RookieCartingClub.Ui;
using Unity.Collections;
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
            RequireForUpdate<CartData>();
            RequireForUpdate<CameraData>();
        }

        protected override void OnUpdate()
        {
            var raceStateEntity = SystemAPI.GetSingletonEntity<TrackPositionsCollection>(); //rc is Race Control
            var raceState = CheckedStateRef.EntityManager.GetComponentObject<RaceState>(raceStateEntity);

            (LocalVelocity velocity, LocalToWorld position, CartData data) playerData = GetCurrentPlayerData();
            
            UI.Instance.Cart = raceState.Racers.Find(i => i.PlayerId == playerData.data.PlayerId);
            UI.Instance.VelocityProvider = new ConstantVelocityProvider { Velocity = playerData.velocity.Velocity };
            MapHandle.Instance.CartPosition = playerData.position.Position;
            MapHandle.Instance.MoveSelf();
            
            if (UI.Instance.Buttons.IsRecordSwitchRequest)
                SwitchRecord(UI.Instance.Recording);
            
            if (UI.Instance.Buttons.InPitRequest)
                SendInPitRequest();


            UI.Instance.UpdateUI();

        }

        private (LocalVelocity velocity, LocalToWorld position, CartData data)
            GetCurrentPlayerData()
        {
            var camera = SystemAPI.GetSingleton<CameraData>();
            var playerEntity = camera.PlayerEntity;
            return (EntityManager.GetComponentData<LocalVelocity>(playerEntity), EntityManager.GetComponentData<LocalToWorld>(playerEntity), EntityManager.GetComponentData<CartData>(playerEntity));
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