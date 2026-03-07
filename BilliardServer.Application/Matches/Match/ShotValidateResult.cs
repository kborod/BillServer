namespace BilliardServer.Application.Matches.Match
{
    public enum ShotValidateResult
    {
        None,
        Ok,
        Player1Desync,
        Player2Desync,
        FullDesync
    }
}
