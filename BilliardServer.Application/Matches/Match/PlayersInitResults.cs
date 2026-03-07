namespace BilliardServer.Application.Matches.Match
{
    public class PlayersInitResults
    {
        public bool Player1Inited { get; private set; }
        public bool Player2Inited { get; private set; }

        public void SetPlayer1Inited()
        {
            Player1Inited = true;
        }

        public void SetPlayer2Inited()
        {
            Player2Inited = true;
        }

        public bool IsAllClientsInited() => Player1Inited && Player2Inited;

    }
}
