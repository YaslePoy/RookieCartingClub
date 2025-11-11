using System;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

[UpdateAfter(typeof(PhysicsSimulationGroup))]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial struct LapRegisterSystem : ISystem
{
    private ComponentLookup<CartData> _cartLookup;
    private ComponentLookup<CheckPointData> _checkPointLookup;
    private BufferLookup<CurrentContactingSegment> _currentContactingSegmentLookup;
    private BufferLookup<NewContactingSegment> _newContactingSegmentLookup;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
        _cartLookup = state.GetComponentLookup<CartData>(true);
        _checkPointLookup = state.GetComponentLookup<CheckPointData>(true);
        _currentContactingSegmentLookup = state.GetBufferLookup<CurrentContactingSegment>();
        _newContactingSegmentLookup = state.GetBufferLookup<NewContactingSegment>();
    }

    // [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _cartLookup.Update(ref state);
        _checkPointLookup.Update(ref state);
        _currentContactingSegmentLookup.Update(ref state);
        _newContactingSegmentLookup.Update(ref state);
        var cols = new NativeList<CartCollision>(Allocator.TempJob);

        var checkJob = new CartPositionHandlingJob
        {
            CartLookup = _cartLookup,
            CheckPointLookup = _checkPointLookup,
            Collisions = cols
        };
        checkJob.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency).Complete();

        
        
        var placementBufferEntities = SystemAPI.QueryBuilder().WithAll<CurrentContactingSegment>().Build()
            .ToEntityArray(Allocator.Temp);
        var placementBuffers =
            new NativeHashMap<Entity, DynamicBuffer<CurrentContactingSegment>>(placementBufferEntities.Length,
                Allocator.Temp);        
        var newBuffers =
            new NativeHashMap<Entity, DynamicBuffer<NewContactingSegment>>(placementBufferEntities.Length,
                Allocator.Temp);

        var currentPlacement = new NativeHashSet<EntitySegment>(4, Allocator.Temp);
        
        foreach (var entity in placementBufferEntities)
        {
            var currentContactingSegments = _currentContactingSegmentLookup[entity];
            placementBuffers[entity] = currentContactingSegments;
            newBuffers[entity] = _newContactingSegmentLookup[entity];
            foreach (var currentContactingSegment in currentContactingSegments)
            {
                currentPlacement.Add(new EntitySegment(entity, currentContactingSegment.Index));
            }
            currentContactingSegments.Clear();
        }


        foreach (var collision in cols)
        {
            var entityBuffer = placementBuffers[collision.PlayerEntity];
            entityBuffer.Add(new CurrentContactingSegment { Index = collision.SegmentId });

            if (!currentPlacement.Contains(new EntitySegment(collision.PlayerEntity, collision.SegmentId)))
            {
                _newContactingSegmentLookup[collision.PlayerEntity].Add(new NewContactingSegment{ Index = collision.SegmentId });
            }
        }

        placementBufferEntities.Dispose();
        placementBuffers.Dispose();
        newBuffers.Dispose();
        currentPlacement.Dispose();
        cols.Dispose();
    }

    public struct EntitySegment : IEquatable<EntitySegment>
    {
        public Entity Entity;
        public int Index;

        public EntitySegment(Entity entity, int index)
        {
            Entity = entity;
            Index = index;
        }

        public bool Equals(EntitySegment other)
        {
            return Entity.Equals(other.Entity) && Index == other.Index;
        }

        public override bool Equals(object obj)
        {
            return obj is EntitySegment other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Entity, Index);
        }
    }
    
    public struct CartPositionHandlingJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentLookup<CartData> CartLookup;
        [ReadOnly] public ComponentLookup<CheckPointData> CheckPointLookup;
        public NativeList<CartCollision> Collisions;

        public void Execute(TriggerEvent triggerEvent)
        {
            var segment = new CheckPointData { Index = -1 };
            var cart = new CartData { PlayerId = -1 };
            var cartEntity = Entity.Null;
            if (CartLookup.TryGetComponent(triggerEvent.EntityA, out var cartDataA))
            {
                cart = cartDataA;
                cartEntity = triggerEvent.EntityA;
            }
            else if (CartLookup.TryGetComponent(triggerEvent.EntityB, out var cartDataB))
            {
                cart = cartDataB;
                cartEntity = triggerEvent.EntityB;
            }
            else
            {
                return;
            }

            if (segment.Index == -1 && cart.PlayerId == -1)
            {
                return;
            }

            if (CheckPointLookup.TryGetComponent(triggerEvent.EntityA, out var checkA))
            {
                segment = checkA;
            }
            else if (CheckPointLookup.TryGetComponent(triggerEvent.EntityB, out var checkB))
            {
                segment = checkB;
            }
            else
            {
                return;
            }

            Collisions.Add(new CartCollision
            {
                PlayerEntity = cartEntity,
                PlayerId = cart.PlayerId,
                SegmentId = segment.Index
            });
        }
    }

    public struct CartCollision
    {
        public Entity PlayerEntity;
        public int PlayerId;
        public int SegmentId;
    }
}