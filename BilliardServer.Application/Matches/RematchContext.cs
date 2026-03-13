using Kborod.BilliardCore.Enums;

namespace BilliardServer.Application.Matches
{
    public class RematchContext
    {
        public string Player1 { get; }
        public string Player2 { get; }
        public string TurningPlayer { get; }
        public GameType GameType { get; }
        public BetType BetType { get; }
        public long EndWaitTimestamp { get; }

        public bool Player1Ready { get; private set; }
        public bool Player2Ready { get; private set; }

        public RematchContext(string player1, string player2, string turningPlayer, GameType gameType, BetType betType, long endWaitTimestamp)
        {
            Player1 = player1;
            Player2 = player2;
            TurningPlayer = turningPlayer;
            GameType = gameType;
            BetType = betType;
            EndWaitTimestamp = endWaitTimestamp;
        }

        public void SetPlayerReady(string playerId)
        {
            if (Player1 == playerId)
                Player1Ready = true;
            else
                Player2Ready = true;
        }

        public string GetOpponent(string playerId)
        {
            return Player1 == playerId ? Player2 : Player1;
        }

        public bool IsOppReady(string playerId)
        {
            return Player1 == playerId ? Player2Ready : Player1Ready;
        }
    }
}
