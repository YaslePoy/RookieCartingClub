using RookieCartingClub.Authoring;
using RookieCartingClub.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace RookieCartingClub.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateAfter(typeof(EnableCartSimulationSystem))]
    public partial class CartPlacementSystem : SystemBase
    {
        private BufferLookup<TrackPlacementPosition> _positionsLookup;

        protected override void OnCreate()
        {
            CheckedStateRef.RequireForUpdate<TrackPositionsCollection>();
            CheckedStateRef.RequireForUpdate<TrackPlacementRequest>();
            _positionsLookup = CheckedStateRef.GetBufferLookup<TrackPlacementPosition>();
        }

        protected override void OnUpdate()
        {
            _positionsLookup.Update(ref CheckedStateRef);

            var positionsCollections = SystemAPI.GetSingletonBuffer<TrackPositionsCollection>().AsNativeArray();

            var rcEntity = SystemAPI.GetSingletonEntity<TrackPositionsCollection>(); //rc is Race Control
            var raceControl = CheckedStateRef.EntityManager.GetComponentObject<RaceControl>(rcEntity);

            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            var job = new CartPlacementJob
            {
                PositionsLookup = _positionsLookup,
                CommandBuffer = ecb,
                PositionsCollection = positionsCollections,
                CurrentRacePeriod = raceControl.CurrentRacePeriod,
            };

            foreach (var (transform, velocity, request, enabledRequest, simulate, cartData, entity) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRW<PhysicsVelocity>, RefRO<TrackPlacementRequest>, EnabledRefRW<TrackPlacementRequest>, EnabledRefRW<Simulate>, RefRO<CartData>>().WithEntityAccess())
            {
                job.Execute(entity, ref transform.ValueRW, ref velocity.ValueRW, request.ValueRO, enabledRequest, simulate, cartData.ValueRO);
            }
            
            ecb.Playback(CheckedStateRef.EntityManager);
            ecb.Dispose();
        }

        public partial struct CartPlacementJob : IJobEntity
        {
            public IRacePeriod CurrentRacePeriod;
            public EntityCommandBuffer CommandBuffer;
            public BufferLookup<TrackPlacementPosition> PositionsLookup;
            public NativeArray<TrackPositionsCollection> PositionsCollection;

            public void Execute(Entity entity,
                ref LocalTransform transform,
                ref PhysicsVelocity velocity,
                TrackPlacementRequest request,
                EnabledRefRW<TrackPlacementRequest> enabledRequest,
                EnabledRefRW<Simulate> simulate,
                in CartData cartData)
            {
                var location = PositionsLookup[PositionsCollection[request.CollectionId].BufferEntity];

                var playerPositionIndex = CurrentRacePeriod.GetPlayerPosition(cartData.PlayerId);

                var position = location[playerPositionIndex];
                transform.Position = position.Position;
                transform.Rotation = position.Rotation;

                velocity.Linear = float3.zero;
                velocity.Angular = float3.zero;

                CommandBuffer.SetComponentEnabled<EnableSimulate>(entity, true);

                simulate.ValueRW = false;
                enabledRequest.ValueRW = false;
            }
        }
    }


    public struct EnableSimulate : IComponentData, IEnableableComponent
    {
    }
}