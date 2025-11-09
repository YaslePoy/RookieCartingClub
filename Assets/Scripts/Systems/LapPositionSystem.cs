using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;


[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
public partial class LapPositionSystem : SystemBase
{
    public ComponentLookup<CartData> CartLookup;
    public ComponentLookup<CheckPointData> CheckPointLookup;

    protected override void OnCreate()
    {
        CartLookup = GetComponentLookup<CartData>(true);
        CheckPointLookup = GetComponentLookup<CheckPointData>(true);
    }

    protected override void OnUpdate()
    {
        CartLookup.Update(ref CheckedStateRef);
        CheckPointLookup.Update(ref CheckedStateRef);
        
        var cols = new NativeList<CartCollision>(Allocator.TempJob);

        var checkJob = new CartPositionHandlingJob
        {
            cartLookup = CartLookup,
            checkPointLookup = CheckPointLookup,
            Collisions = cols
        };
        checkJob.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), Dependency).Complete();


        if (cols.Length > 0)
        {
            Debug.Log("Some cols");
        }
        for (int i = 0; i < cols.Length; i++)
        {
            Debug.Log($"Player [{cols[i].PlayerId}] Segment [{cols[i].SegmentId, 4}]");
        }

        cols.Dispose();
    }

    public partial struct CartPositionHandlingJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentLookup<CartData> cartLookup;
        [ReadOnly] public ComponentLookup<CheckPointData> checkPointLookup;
        public NativeList<CartCollision> Collisions;

        public void Execute(TriggerEvent triggerEvent)
        {
            
            var segment = new CheckPointData { Index = -1 };
            var cart = new CartData { PlayerId = -1 };
            
            if (cartLookup.TryGetComponent(triggerEvent.EntityA, out var cartDataA))
            {
                cart = cartDataA;
            }
            else if (cartLookup.TryGetComponent(triggerEvent.EntityB, out var cartDataB))
            {
                cart = cartDataB;
            }
            else
            {
                return;
            }

            if (segment.Index == -1 && cart.PlayerId == -1)
            {
                return;
            }

            if (checkPointLookup.TryGetComponent(triggerEvent.EntityA, out var checkA))
            {
                segment = checkA;
            }
            else if (checkPointLookup.TryGetComponent(triggerEvent.EntityB, out var checkB))
            {
                segment = checkB;
            }
            else
            {
                return;
            }

            Collisions.Add(new CartCollision
            {
                PlayerId = cart.PlayerId,
                SegmentId = segment.Index
            });
        }
    }

    public struct CartCollision
    {
        public int PlayerId;
        public int SegmentId;
    }
}