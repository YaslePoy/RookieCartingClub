using System.Collections.Generic;
using RookieCartingClub.Authoring;
using Unity.Entities;

namespace RookieCartingClub.Components
{
    public class RaceState : IComponentData
    {
        public IRacePeriod CurrentRacePeriod = new PracticePeriod();
        public Queue<IRacePeriod> RacePeriods = new();

        public TrackPositions TrackPositions = new();
        public List<CartHandle> Racers = new();
    }
}