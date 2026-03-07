using Kborod.BilliardCore.Rules;
using Kborod.BilliardCore.Rules.PoolEight;

namespace Kborod.BilliardCore
{
    public class PoolEightTurnResults : ITurnResult
    {
        public PoolEightRulesShotResult PoolEightRulesResult { get; private set; }
        public RulesShotResult RulesResult { get; private set; }
        public TurnSettings NextTurnSettings { get; private set; }

        public PoolEightTurnResults(PoolEightRulesShotResult poolEightRulesResult, RulesShotResult rulesResult, TurnSettings turnSettings)
        {
            PoolEightRulesResult = poolEightRulesResult;
            RulesResult = rulesResult;
            NextTurnSettings = turnSettings;
        }
    }
}
