using System.Linq;
using RookieCartingClub.Components;
using Unity.Entities;

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
            var raceStateEntity = SystemAPI.GetSingletonEntity<TrackPositionsCollection>(); //rc is Race Control
            var raceState = CheckedStateRef.EntityManager.GetComponentObject<RaceState>(raceStateEntity);
            
            var time = SystemAPI.Time.ElapsedTime;
            
            if (raceState.Racers.Count == 0)
                return;
            
            foreach (var (id, buffer) in SystemAPI.Query<RefRO<CartData>, DynamicBuffer<NewContactingSegment>>())
            {
                var handle = raceState.Racers.First(i => i.PlayerId == id.ValueRO.PlayerId);
            
                if (buffer.IsEmpty)
                    continue;

                foreach (var segment in buffer)
                {
                    handle.PushCheckPoint(new CheckPointData
                    {
                        Index = segment.Index
                    }, time);
                }
                
                buffer.Clear();
            }
        }
    }
}