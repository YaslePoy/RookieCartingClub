using System;
using System.IO;
using RookieCartingClub.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace RookieCartingClub.Systems.Replay
{
    [UpdateInGroup(typeof(CartPhysicsSimulationGroup))]
    [UpdateBefore(typeof(InputPhysicalImplementationSystem))]
    public partial class LoggingSystem : SystemBase
    {
        private NativeList<SessionStamp> _stamps;
        private EntityQuery _cartQuery;

        protected override void OnCreate()
        {
            _stamps = new NativeList<SessionStamp>(1024, Allocator.Persistent);
            _cartQuery = SystemAPI.QueryBuilder().WithAll<CartInputData>().WithNone<Prefab>().Build();
        }

        protected override void OnUpdate()
        {
            if (_cartQuery.CalculateEntityCount() == 0 && _stamps.IsEmpty == false)
            {
                Enabled = false;
                SaveStamps();
                return;
            }

            if (_cartQuery.CalculateEntityCount() == 0)
            {
                return;
            }

            WriteStamp();
        }

        private void SaveStamps()
        {
            using var file = File.OpenWrite($"{DateTime.Now.ToFileTimeUtc()}.csv");
            using var writer = new StreamWriter(file);
            writer.WriteLine("Time;SteerAngle;EngineForce;Speed;Force;Rotation;ForceVector;Wheel0;Wheel1;Wheel2;Wheel3");
            foreach (var stamp in _stamps)
            {
                writer.WriteLine(
                    $"{stamp.Time};{stamp.SteerAngle};{stamp.EngineForce};{stamp.Speed};{stamp.Force};{stamp.Rotation};{stamp.ForceVector};{stamp.LocalSpeed0};{stamp.LocalSpeed1};{stamp.LocalSpeed2};{stamp.LocalSpeed3}");
            }
        }

        private void WriteStamp()
        {
            var player = _cartQuery.ToEntityArray(Allocator.Temp)[0];
            var stamp = new SessionStamp
            {
                Time = SystemAPI.Time.ElapsedTime
            };

            var forceComponent = EntityManager.GetComponentData<ForceApplier>(player);

            stamp.Force = math.length(forceComponent.LogForce);
            stamp.Rotation = math.length(forceComponent.LogRotation) * math.TODEGREES;
            stamp.ForceVector = math.atan2(forceComponent.LogForce.x, forceComponent.LogForce.z);
            stamp.EngineForce = EntityManager.GetComponentData<EngineData>(player).CurrentForce;
            stamp.Speed = math.length(EntityManager.GetComponentData<PhysicsVelocity>(player).Linear);

            var input = EntityManager.GetComponentData<CartInputData>(player);

            stamp.SteerAngle = input.CurrentAngle;

            var children = EntityManager.GetBuffer<Child>(player).ToNativeArray(Allocator.Temp);
            foreach (var child in children)
            {
                if (EntityManager.HasComponent<CartWheel>(child.Value) == false)
                    continue;
                
                var velocity = EntityManager.GetComponentData<LocalVelocity>(child.Value);
                switch (velocity.Index)
                {
                    case 0:
                        stamp.LocalSpeed0 = math.length(velocity.Velocity);
                        break;
                    case 1:
                        stamp.LocalSpeed1 = math.length(velocity.Velocity);
                        break;
                    case 2:
                        stamp.LocalSpeed2 = math.length(velocity.Velocity);
                        break;
                    case 3:
                        stamp.LocalSpeed3 = math.length(velocity.Velocity);
                        break;
                }
            }

            _stamps.Add(stamp);
        }
    }

    public struct SessionStamp
    {
        public double Time;
        public float SteerAngle;
        public float Speed;
        public float Force;
        public float ForceVector;
        public float Rotation;
        public float EngineForce;
        public float LocalSpeed0;
        public float LocalSpeed1;
        public float LocalSpeed2;
        public float LocalSpeed3;
    }
}