using Kborod.BilliardCore.Enums;

namespace BilliardServer.Application.Matches
{
    public class CreateMatchContext
    {
        public string Id { get; }
        public string Player1 { get; }
        public string Player2 { get; }
        public string TurningPlayer { get; }
        public GameType GameType { get; }
        public BetType BetType { get; }
        public int PosNum { get; }

        public CreateMatchContext(string id, string player1, string player2, string turningPlayer, GameType gameType, BetType betType, int posNum)
        {
            Id = id;
            Player1 = player1;
            Player2 = player2;
            TurningPlayer = turningPlayer;
            GameType = gameType;
            BetType = betType;
            PosNum = posNum;
        }
    }
}
