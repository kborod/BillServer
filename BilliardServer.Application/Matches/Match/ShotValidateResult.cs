namespace BilliardServer.Application.Matches.Match
{
    public enum ShotValidateResult
    {
        None,
        Ok,
        DesyncServerWithFront,
        Player1Desync,
        Player2Desync,
        FullDesync
    }
}
