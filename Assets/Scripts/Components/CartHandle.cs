using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RookieCartingClub.Authoring;
using Unity.Entities;
using UnityEngine;

namespace RookieCartingClub.Components
{
    public class CartHandle : IComponentData
    {
        public int PlayerId;
        public List<TrackLap> Laps = new();
        public TrackLap CurrentLap => Laps.LastOrDefault();
        [CanBeNull]
        public TrackLap FastestLaps => Laps.OrderBy(i => i.TotalLapTime).FirstOrDefault(i => i.IsValid && i.IsFinished);
        public int CheckCount;
        public void PushCheckPoint(CheckPointData checkPoint, double time)
        {
            var currentLap = CurrentLap;

            if (checkPoint.Index == 0)
                Laps.Add(new TrackLap(CheckCount, time));
            else
                currentLap.SetupSegmentTime(time, checkPoint.Index);
        }
        
        public CartHandle()
        {
        }

        public void Init()
        {
            Laps.Add(new TrackLap(CheckCount, Time.timeAsDouble));
        }
    }
}