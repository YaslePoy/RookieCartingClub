namespace RookieCartingClub.Authoring
{
    public interface IRacePeriod
    {
        void Start(RaceControlAuthoring raceControlAuthoring);
        void Update(RaceControlAuthoring raceControlAuthoring);
        int GetPlayerPosition(int playerId);
    }
}