using Kborod.BilliardCore.Enums;

namespace BilliardServer.Application.Matches
{
    //public bool needChoosePocketAllTime;
    //public bool needChoosePocketAtFinalShot;
    //public int shotTime;
    //public bool showAimLine;
    //public int tableId;
    //public bool minTwoParties;

    public static class ExpConfig
    {
        private static Dictionary<BetType, BetParams> betTypeParams = new ()
        {
            { BetType.None,  new BetParams(0, 0, 0, 0) },
            { BetType.PVP_100,  new BetParams(100, 200, 21, 4) },       //Зеленый стол
            { BetType.PVP_100,  new BetParams(500, 1000, 59, 12) },       //Синий стол
            { BetType.PVP_100,  new BetParams(1000, 2000, 94, 21) },       //Красный стол

            //put(GamesManager.EnumGameType.PVP_100,      new GameTypeParams(false, false, 30000, true, "Зеленый стол",   100,    200,    21,     4,  1, false));
            //put(GamesManager.EnumGameType.PVP_500,      new GameTypeParams(false, false, 28000, true, "Синий стол",     500,    1000,   59,     12, 2, false));
            //put(GamesManager.EnumGameType.PVP_1000,     new GameTypeParams(false, false, 27000, true, "Красный стол",   1000,   2000,   94,     21, 3, true));
            //put(GamesManager.EnumGameType.PVP_5000,     new GameTypeParams(false, true, 26000, true, "Серый стол",      5000,   10000,  191,    46, 4, true));
            //put(GamesManager.EnumGameType.PVP_20000,    new GameTypeParams(false, true, 25000, true, "Черный стол",     20000,  40000,  319,    74, 5, true));
            //put(GamesManager.EnumGameType.PVP_100000,   new GameTypeParams(true, true, 24000, true, "Оранжевый стол",   100000, 200000, 510,    124, 6, true));
            //put(GamesManager.EnumGameType.PVP_200000,   new GameTypeParams(true, true, 23000, true, "Винный стол",      200000, 400000, 963,    245, 7, true));
            //put(GamesManager.EnumGameType.PVP_500000,   new GameTypeParams(false, true, 22000, false, "Шартрез",        500000, 1000000, 1352,  349, 8, true));
            //put(GamesManager.EnumGameType.PVP_1000000,  new GameTypeParams(true, true, 20000, false, "Золотой стол",    1000000, 2000000, 1500, 400, 9, true));
        };

        public static BetParams GetBetParams(BetType betType)
        {
            if (betTypeParams.TryGetValue(betType, out var res))
                return res;
            else
                throw new Exception($"Bet {betType} config not found");
        }
    }

    public class BetParams
    {
        public int Bet { get; }
        public int Prize { get; }
        public int WinnerExp { get; }
        public int LooserExp { get; }

        public BetParams(int bet, int prize, int winnerExp, int looserExp)
        {
            Bet = bet;
            Prize = prize;
            WinnerExp = winnerExp;
            LooserExp = looserExp;
        }
    }
}
