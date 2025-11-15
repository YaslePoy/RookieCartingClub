using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateAfter(typeof(PhysicsSimulationGroup))]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial struct LapRegisterSystem : ISystem
{
    private ComponentLookup<CartData> _cartLookup;
    private ComponentLookup<CheckPointData> _checkPointLookup;
    private BufferLookup<NewContactingSegment> _newContactingSegmentLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
        _cartLookup = state.GetComponentLookup<CartData>(true);
        _checkPointLookup = state.GetComponentLookup<CheckPointData>(true);
        _newContactingSegmentLookup = state.GetBufferLookup<NewContactingSegment>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _cartLookup.Update(ref state);
        _checkPointLookup.Update(ref state);
        _newContactingSegmentLookup.Update(ref state);
        var cols = new NativeList<CartCollision>(Allocator.TempJob);

        var checkJob = new CartPositionHandlingJob
        {
            CartLookup = _cartLookup,
            CheckPointLookup = _checkPointLookup,
            Collisions = cols
        };
        checkJob.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency).Complete();

        var placementBuffers = new NativeHashMap<Entity, DynamicBuffer<CurrentContactingSegment>>(32, Allocator.Temp);
        var newBuffers = new NativeHashMap<Entity, DynamicBuffer<NewContactingSegment>>(32, Allocator.Temp);

        var currentPlacement = new NativeHashSet<EntitySegment>(4, Allocator.Temp);

        foreach (var (currentContactingSegments, entity)
                 in SystemAPI.Query<DynamicBuffer<CurrentContactingSegment>>().WithEntityAccess())
        {
            FillMapsAndBuffer(placementBuffers, entity, currentContactingSegments, newBuffers, currentPlacement);
        }


        foreach (var collision in cols) RegisterNewSegments(placementBuffers, collision, currentPlacement);

        placementBuffers.Dispose();
        newBuffers.Dispose();
        currentPlacement.Dispose();
        cols.Dispose();
    }

    private void RegisterNewSegments(NativeHashMap<Entity, DynamicBuffer<CurrentContactingSegment>> placementBuffers,
        CartCollision collision,
        NativeHashSet<EntitySegment> currentPlacement)
    {
        var entityBuffer = placementBuffers[collision.PlayerEntity];
        entityBuffer.Add(new CurrentContactingSegment { Index = collision.SegmentId });

        if (!currentPlacement.Contains(new EntitySegment(collision.PlayerEntity, collision.SegmentId)))
        {
            _newContactingSegmentLookup[collision.PlayerEntity]
                .Add(new NewContactingSegment { Index = collision.SegmentId });
        }
    }

    private void FillMapsAndBuffer(NativeHashMap<Entity, DynamicBuffer<CurrentContactingSegment>> placementBuffers,
        Entity entity, DynamicBuffer<CurrentContactingSegment> currentContactingSegments,
        NativeHashMap<Entity, DynamicBuffer<NewContactingSegment>> newBuffers,
        NativeHashSet<EntitySegment> currentPlacement)
    {
        placementBuffers[entity] = currentContactingSegments;
        newBuffers[entity] = _newContactingSegmentLookup[entity];
        foreach (var currentContactingSegment in currentContactingSegments)
            currentPlacement.Add(new EntitySegment(entity, currentContactingSegment.Index));

        currentContactingSegments.Clear();
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

    [BurstCompile]
    public struct CartPositionHandlingJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentLookup<CartData> CartLookup;
        [ReadOnly] public ComponentLookup<CheckPointData> CheckPointLookup;
        public NativeList<CartCollision> Collisions;

        public void Execute(TriggerEvent triggerEvent)
        {
            CheckPointData segment;
            Entity cartEntity;
            
            if (CartLookup.HasComponent(triggerEvent.EntityA))
                cartEntity = triggerEvent.EntityA;
            else if (CartLookup.HasComponent(triggerEvent.EntityB))
                cartEntity = triggerEvent.EntityB;
            else
                return;
            
            if (CheckPointLookup.TryGetComponent(triggerEvent.EntityA, out var checkA))
                segment = checkA;
            else if (CheckPointLookup.TryGetComponent(triggerEvent.EntityB, out var checkB))
                segment = checkB;
            else
                return;

            Collisions.Add(new CartCollision
            {
                PlayerEntity = cartEntity,
                SegmentId = segment.Index
            });
        }
    }

    public struct CartCollision
    {
        public Entity PlayerEntity;
        public int SegmentId;
    }
}