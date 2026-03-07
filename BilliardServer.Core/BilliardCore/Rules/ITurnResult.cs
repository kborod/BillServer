using Kborod.BilliardCore.Enums;
using System.Collections.Generic;

namespace Kborod.BilliardCore.Rules
{
    public interface ITurnResult
    {
        public RulesShotResult RulesResult { get;  }
        public TurnSettings NextTurnSettings { get; }
    }
}
