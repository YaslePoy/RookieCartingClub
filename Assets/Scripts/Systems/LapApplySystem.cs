using System.Linq;
using Unity.Collections;
using Unity.Entities;

public partial class LapApplySystem : SystemBase
{
    private BufferLookup<NewContactingSegment> _bufferLookup;
    private ComponentLookup<CartData> _dataLookup;

    protected override void OnCreate()
    {
        _bufferLookup = GetBufferLookup<NewContactingSegment>();
        _dataLookup = GetComponentLookup<CartData>();
        RaceControl.Singleton = new RaceControl();
        var cartHandle = new CartHandle { CheckCount = 939 };
        cartHandle.Start();
        RaceControl.Singleton.racers.Add(cartHandle);
    }

    protected override void OnUpdate()
    {
        _bufferLookup.Update(ref CheckedStateRef);
        _dataLookup.Update(ref CheckedStateRef);

        using var carts = SystemAPI.QueryBuilder().WithAll<NewContactingSegment>().Build()
            .ToEntityArray(Allocator.Temp);

        foreach (var cart in carts)
        {
            var id = _dataLookup[cart];
            var buffer = _bufferLookup[cart];
            var handle = RaceControl.Singleton.racers.First(i => i.PlayerId.Value == id.PlayerId);

            if (buffer.IsEmpty)
                continue;
            
            foreach (var segment in buffer)
            {
                handle.PushCheckPoint(new CheckPointData
                {
                    Index = segment.Index,
                });
            }

            buffer.Clear();
        }
    }
}