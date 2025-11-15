using System;
using UnityEngine;

namespace RookieCartingClub.Authoring
{
    public class TrackLap
    {
        public static readonly TrackLap Null = new(0, 0);
        public readonly double LapStart;
        public readonly double[] SegmentTimes;

        public TrackLap(int lastSegments, double lapStart)
        {
            Debug.Log("New lap started");
            LapStart = lapStart;
            SegmentTimes = new double[lastSegments];
        }

        public bool IsFinished => LastSegmentIndex == SegmentTimes.Length;
        public bool IsValid { get; private set; } = true;

        public int LastSegmentIndex { get; private set; } = 1;

        public double TotalLapTime => IsFinished ? SegmentTimes[^1] : 0;

        public double Delta(TrackLap other)
        {
            return SegmentTimes[LastSegmentIndex - 1] - other.SegmentTimes[LastSegmentIndex - 1];
        }

        public void SetupSegmentTime(double time, int segmentIndex)
        {
            if (segmentIndex == LastSegmentIndex)
            {
                SegmentTimes[LastSegmentIndex++] = time - LapStart;

                if (IsFinished) Debug.Log($"Lap finished: {TimeSpan.FromSeconds(SegmentTimes[^1])}; Valid: {IsValid}");
                return;
            }

            if (IsValid) Debug.LogWarning("Lap invalidated!");

            IsValid = false;

            if (segmentIndex > LastSegmentIndex)
                for (var i = LastSegmentIndex; i <= segmentIndex; i++)
                    SegmentTimes[i] = time - LapStart;

            LastSegmentIndex = segmentIndex + 1;
        }
    }
}