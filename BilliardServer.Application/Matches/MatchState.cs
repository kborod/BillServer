namespace BilliardServer.Application.Matches
{
    public enum MatchState
    {
        Initializing,
        WaitingPlayersInit,
        PrepeareTurn,
        WaitingTurnResults,
        Over,
        ShotValidationError,
    }
}
