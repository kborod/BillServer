using Kborod.BilliardCore.Enums;
using System.Collections.Generic;

namespace Kborod.BilliardCore.Rules
{
    public class RulesShotResult
    {
        public FoulType Foul { get; private set; } = FoulType.None;

        /// <summary> Состояние шаров (после удара и применения правил) </summary
        public List<BallData> BallDatas { get; private set; }

        /// <summary> Все забитые в процессе удара шары </summary
        public List<int> PocketedBalls { get; private set; }

        /// <summary>  Шары, возвращенные после удара (после применения правил)  </summary>
        public List<int> ReturnedPocketedBalls { get; private set; }

        /// <summary> Игрок, который производил удар</summary>
        public string CurrTurnPlayerId{ get; private set; }

        /// <summary> Игрок, который будет производить удар в следующий ход </summary>
        public string NextTurnPlayerId{ get; private set; }

        /// <summary> Победитель, если определен </summary>
        public string WinUserIdOrNull { get; private set; }

        public RulesShotResult(
            FoulType foul,
            List<BallData> ballDatas,
            List<int> pocketedBalls,
            List<int> returnedPocketedBalls, 
            string currTurnPlayerId,
            string nextTurnPlayerId, 
            string winUserIdOrNull)
        {
            Foul = foul;
            BallDatas = ballDatas;
            PocketedBalls = pocketedBalls;
            ReturnedPocketedBalls = returnedPocketedBalls;
            CurrTurnPlayerId = currTurnPlayerId;
            NextTurnPlayerId = nextTurnPlayerId;
            WinUserIdOrNull = winUserIdOrNull;
        }

        public string GetDifferences(RulesShotResult other)
        {
            var result = string.Empty;
            if (Foul != other.Foul)
                result += $"Fouls: {Foul} - {other.Foul}\n";

            if (NextTurnPlayerId != other.NextTurnPlayerId)
                result += $"NextTurnPlayerId: {NextTurnPlayerId} - {other.NextTurnPlayerId}\n";

            if (WinUserIdOrNull != other.WinUserIdOrNull)
                result += $"WinUserIdOrNull: {WinUserIdOrNull} - {other.WinUserIdOrNull}\n";

            if (BallDatas.Count != other.BallDatas.Count)
            {
                result += $"BallDatas.Count: {BallDatas.Count} - {other.BallDatas.Count}\n";
                return result;
            }

            foreach (var b1 in BallDatas)
            {
                var b2 = other.BallDatas[b1.Number];

                if (b1.IsRemoved != b2.IsRemoved || (b1.IsRemoved == false && (b1.Xraw != b2.Xraw || b1.Yraw != b2.Yraw)))
                    result += $"{b1} - {b2}\n";
            }

            return result;
        }
    }
}
