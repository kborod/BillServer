using Kborod.BilliardCore.Rules;
using System.Text;

namespace BilliardServer.Application.Matches.Match
{
    public class MakeShotResults
    {
        public ITurnResult? CalculatedResult { get; private set; }
        private RulesShotResult? Player1Result { get; set; }
        private RulesShotResult? Player2Result { get; set; }

        public ShotValidateResult LastValidateResult { get; private set; } = ShotValidateResult.None;
        public string DesyncLog { get; private set; } = string.Empty;

        public void SetCalculateResult(ITurnResult result)
        {
            CalculatedResult = result; 
        }

        public void SetPlayer1Result(RulesShotResult result)
        {
            Player1Result = result;
        }

        public void SetPlayer2Result(RulesShotResult result)
        {
            Player2Result = result;
        }

        public bool IsAllResultsReceived() => CalculatedResult != null && Player1Result != null && Player2Result != null;

        public ShotValidateResult Validate()
        {
            if (IsAllResultsReceived() == false)
                throw new Exception("[MakeShotResults] Not all results received");

            //Отличие результатов между клиентами
            if (HasDifference(Player1Result!, Player2Result!))
            {
                if (HasDifference(Player1Result!, CalculatedResult!.RulesResult!) == false)
                {
                    LastValidateResult = ShotValidateResult.Player2Desync;
                }
                else if (HasDifference(Player2Result!, CalculatedResult!.RulesResult!) == false)
                {
                    LastValidateResult = ShotValidateResult.Player1Desync;
                }
                else
                {
                    LastValidateResult = ShotValidateResult.FullDesync;
                }
            }
            else if (HasDifference(Player1Result!, CalculatedResult!.RulesResult!))
            {
                LastValidateResult =  ShotValidateResult.DesyncServerWithFront;
            }
            else
            {
                LastValidateResult = ShotValidateResult.Ok;
            }

            if(LastValidateResult != ShotValidateResult.Ok)
            {
                var s = new StringBuilder();
                s.AppendLine("Player1 to Player2:");
                s.AppendLine(Player1Result!.GetDifferences(Player2Result!));
                s.AppendLine("Calc to Player1:");
                s.AppendLine(CalculatedResult.RulesResult!.GetDifferences(Player1Result!));
                s.AppendLine("Calc to Player2:");
                s.AppendLine(CalculatedResult.RulesResult!.GetDifferences(Player2Result!));
                DesyncLog = s.ToString();
            }

            return LastValidateResult;
        }

        public void Clear()
        {
            CalculatedResult = null;
            Player1Result = Player2Result = null;
            LastValidateResult = ShotValidateResult.None;
            DesyncLog = string.Empty;
        }

        private bool HasDifference(RulesShotResult r1, RulesShotResult r2)
        {
            var desyncLog = r1.GetDifferences(r2);
            return string.IsNullOrEmpty(desyncLog) == false;
        }
    }
}
