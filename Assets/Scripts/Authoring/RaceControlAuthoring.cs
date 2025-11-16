using System;
using System.Collections.Generic;
using System.Text;
using RookieCartingClub.Components;
using Unity.Entities;
using UnityEngine;

namespace RookieCartingClub.Authoring
{
    public class RaceControlAuthoring : MonoBehaviour
    {
        [Obsolete]
        // public static RaceControlAuthoring Singleton;
        public List<CartHandleAuthoring> racers = new();

        public UIVM Uivm;
        public IRacePeriod CurrentRacePeriod;
        public Queue<IRacePeriod> racePeriods = new();

        public TrackPositions TrackPositions = new();


        private void UpdatePositions()
        {
            var sb = new StringBuilder(256);
            for (var i = 0; i < TrackPositions.Positions.Count; i++)
            {
                var pos = i + 1;
                var nickname = racers.Find(c => c.PlayerId == TrackPositions.Positions[i]).Nickname.Value;
                sb.AppendLine($"{pos,2} | {nickname,-20}");
            }

            Uivm.Positions = sb.ToString();
        }

        public class RaceControlBaker : Baker<RaceControlAuthoring>
        {
            public override void Bake(RaceControlAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var collection = AddBuffer<TrackPositionsCollection>(entity);

                var trackPositions = CreateAdditionalEntity(TransformUsageFlags.None, entityName: "TrackPositions");
                var pitPositions = CreateAdditionalEntity(TransformUsageFlags.None, entityName: "PitPositions");

                collection.Add(new TrackPositionsCollection { BufferEntity = trackPositions });
                collection.Add(new TrackPositionsCollection { BufferEntity = pitPositions });
                var trackBuffer = AddBuffer<TrackPlacementPosition>(trackPositions);
                var pitBuffer = AddBuffer<TrackPlacementPosition>(pitPositions);

                var positions = GameObject.Find("Starts").GetComponentsInChildren<Transform>()[1..];

                foreach (var transform in positions)
                {
                    trackBuffer.Add(new TrackPlacementPosition
                    {
                        Position = transform.position,
                        Rotation = transform.rotation
                    });
                }


                positions = (GameObject.Find("Pitline starts") ?? GameObject.Find("Starts"))
                    .GetComponentsInChildren<Transform>()[1..];

                foreach (var transform in positions)
                {
                    pitBuffer.Add(new TrackPlacementPosition
                    {
                        Position = transform.position,
                        Rotation = transform.rotation
                    });
                }

                AddComponentObject(entity, new RaceControl());
            }
        }
    }
}

// public class PrePeriod : IRacePeriod
// {
//     public double Duration;
//
//     public void Start(RaceControl raceControl)
//     {
//         TrackPlacement.CurrentSpawn = 0;
//         foreach (var racer in raceControl.racers)
//         {
//             racer.GetComponent<TrackPlacement>().PlaceOnTrack();
//             racer.GetComponent<UserControl>().AllowControl = false;
//             racer.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
//             racer.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
//             racer.Laps.Clear();
//         }
//
//         // raceControl.PeriodName.Value = new FixedString32Bytes("Подготовка");
//         // raceControl.PeriodEnd.Value = Duration + Time.timeAsDouble;
//         // raceControl.PeriodType.Value = PeriodType.PreRace;
//         CartHandle.NewCartConnected = handle =>
//         {
//             handle.GetComponent<TrackPlacement>().PlaceOnTrack();
//             handle.GetComponent<UserControl>().AllowControl = false;
//             handle.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
//             handle.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
//         };
//     }
//
//     public void Update(RaceControl raceControl)
//     {
//     }
// }
//
// public class RacePeriod : IRacePeriod
// {
//     public double Duration;
//
//     public void Start(RaceControl raceControl)
//     {
//         foreach (var racer in raceControl.racers) racer.GetComponent<UserControl>().AllowControl = true;
//
//         // raceControl.PeriodType.Value = PeriodType.Race;
//         // raceControl.PeriodName.Value = new FixedString32Bytes("Гонка");
//         // raceControl.PeriodEnd.Value = Duration + Time.timeAsDouble;
//
//         CartHandle.NewCartConnected = handle =>
//         {
//             handle.GetComponent<TrackPlacement>().PlaceInPits();
//             handle.GetComponent<UserControl>().AllowControl = true;
//             handle.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
//             handle.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
//         };
//     }
//
//     public void Update(RaceControl raceControl)
//     {
//         var racersOrder = raceControl.racers.OrderByDescending(i => i.Laps.Count)
//             .ThenBy(i => (i.CurrentLap ?? TrackLap.Null).LastSegmentIndex).ToList();
//         raceControl.racers = racersOrder;
//         raceControl.TrackPositions.Positions = racersOrder.Select(i => i.PlayerId).ToList();
//     }
// }
//
// public class PracticePeriod : IRacePeriod
// {
//     public double Duration;
//
//     public void Start(RaceControl raceControl)
//     {
//         TrackPlacement.CurrentSpawn = 0;
//         foreach (var racer in raceControl.racers)
//         {
//             racer.GetComponent<TrackPlacement>().PlaceInPits();
//             racer.GetComponent<UserControl>().AllowControl = true;
//             var rigidbody = racer.GetComponent<Rigidbody>();
//             rigidbody.linearVelocity = Vector3.zero;
//             rigidbody.angularVelocity = Vector3.zero;
//         }
//
//         // raceControl.PeriodType.Value = PeriodType.Practice;
//         // raceControl.PeriodName.Value = new FixedString32Bytes("Практика");
//         // raceControl.PeriodEnd.Value = Duration + Time.timeAsDouble;
//
//         CartHandle.NewCartConnected = handle =>
//         {
//             handle.GetComponent<TrackPlacement>().PlaceInPits();
//             handle.GetComponent<UserControl>().AllowControl = true;
//             var rigidbody = handle.GetComponent<Rigidbody>();
//             rigidbody.linearVelocity = Vector3.zero;
//             rigidbody.angularVelocity = Vector3.zero;
//         };
//     }
//
//     public void Update(RaceControl raceControl)
//     {
//         var racersOrder = raceControl.racers.OrderByDescending(i => i.Laps.Count)
//             .ThenBy(i => i.CurrentLap.LastSegmentIndex).ToList();
//         raceControl.racers = racersOrder;
//         raceControl.TrackPositions.Positions = racersOrder.Select(i => i.PlayerId).ToList();
//     }
// }
//
// public class FinishPeriod : IRacePeriod
// {
//     public int CurrentLap;
//     public double Duration;
//     public bool IsLeaderFinished;
//
//     public void Start(RaceControl raceControl)
//     {
//         // raceControl.PeriodType.Value = PeriodType.Finish;
//         // raceControl.PeriodName.Value = new FixedString32Bytes("🏁 Финиш");
//         // raceControl.PeriodEnd.Value = Duration + Time.timeAsDouble;
//         if (raceControl.racers.FirstOrDefault() is { } racer) CurrentLap = racer.Laps.Count;
//     }
//
//     public void Update(RaceControl raceControl)
//     {
//         var racersOrder = raceControl.racers.OrderByDescending(i => i.Laps.Count)
//             .ThenBy(i => i.CurrentLap.LastSegmentIndex).ToList();
//         raceControl.racers = racersOrder;
//         raceControl.TrackPositions.Positions = racersOrder.Select(i => i.PlayerId).ToList();
//     }
// }