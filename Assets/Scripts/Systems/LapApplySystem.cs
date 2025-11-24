using System.Linq;
using RookieCartingClub.Authoring;
using RookieCartingClub.Components;
using Unity.Entities;
using Unity.NetCode;

namespace RookieCartingClub.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    
    public partial class LapApplySystem : SystemBase
    {
        protected override void OnCreate()
        {
            CheckedStateRef.RequireForUpdate<TrackPositionsCollection>();
        }

        protected override void OnUpdate()
        {
            var rcEntity = SystemAPI.GetSingletonEntity<TrackPositionsCollection>(); //rc is Race Control
            var raceControl = CheckedStateRef.EntityManager.GetComponentObject<RaceControl>(rcEntity);
            
            var time = SystemAPI.Time.ElapsedTime;
            
            if (raceControl.Racers.Count == 0)
                return;
            
            foreach (var (id, buffer) in SystemAPI.Query<RefRO<CartData>, DynamicBuffer<NewContactingSegment>>())
            {
                var handle = raceControl.Racers.First(i => i.PlayerId == id.ValueRO.PlayerId);
            
                if (buffer.IsEmpty)
                    continue;
            
                foreach (var segment in buffer)
                    handle.PushCheckPoint(new CheckPointData
                    {
                        Index = segment.Index
                    }, time);
            
                buffer.Clear();
            }
        }
    }
}