using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class TrackLap
    {
        public readonly double[] SegmentTimes;
        private int _lastSegmentIndex = 1;
        private bool _isValid = true;
        public static readonly TrackLap Null = new(0, 0);
        public bool IsFinished => _lastSegmentIndex == SegmentTimes.Length;
        public bool IsValid => _isValid;
        public int LastSegmentIndex => _lastSegmentIndex;
        public readonly double LapStart;
        public double TotalLapTime => IsFinished ? SegmentTimes[^1] : 0; 
        public TrackLap(int lastSegments, double lapStart)
        {
            Debug.Log("New lap started");
            LapStart = lapStart;
            SegmentTimes = new double[lastSegments];
        }

        public double Delta(TrackLap other)
        {
            return SegmentTimes[_lastSegmentIndex - 1] - other.SegmentTimes[_lastSegmentIndex - 1];
        }

        public void SetupSegmentTime(double time, int segmentIndex)
        {
            if (segmentIndex == _lastSegmentIndex)
            {
                SegmentTimes[_lastSegmentIndex++] = time - LapStart;

                if (IsFinished)
                {
                    Debug.Log($"Lap finished: {TimeSpan.FromSeconds(SegmentTimes[^1])}; Valid: {IsValid}");
                }
                return;
            }

            if (_isValid)
            {
                Debug.LogWarning("Lap invalidated!");
            }
            
            _isValid = false;

            if (segmentIndex > _lastSegmentIndex)
            {
                for (int i = _lastSegmentIndex; i <= segmentIndex; i++)
                {
                    SegmentTimes[i] = time - LapStart;
                }
            }

            _lastSegmentIndex = segmentIndex + 1;
        }
    }
}