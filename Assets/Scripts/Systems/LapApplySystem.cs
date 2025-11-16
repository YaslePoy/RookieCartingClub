using System.Linq;
using RookieCartingClub.Authoring;
using RookieCartingClub.Components;
using Unity.Entities;

namespace RookieCartingClub.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial class LapApplySystem : SystemBase
    {
        protected override void OnCreate()
        {
            RaceControl.Singleton = new RaceControl();
            var cartHandle = new CartHandle { CheckCount = 939 };
            cartHandle.Start();
            RaceControl.Singleton.racers.Add(cartHandle);
        }

        protected override void OnUpdate()
        {
            foreach (var (id, buffer) in SystemAPI.Query<RefRO<CartData>, DynamicBuffer<NewContactingSegment>>())
            {
                var handle = RaceControl.Singleton.racers.First(i => i.PlayerId == id.ValueRO.PlayerId);

                if (buffer.IsEmpty)
                    continue;

                foreach (var segment in buffer)
                    handle.PushCheckPoint(new CheckPointData
                    {
                        Index = segment.Index
                    });

                buffer.Clear();
            }
        }
    }
}