using Kborod.BilliardCore.Rules;
using System.Collections.Generic;

namespace Kborod.BilliardCore
{
    /// <summary> Информация для синхронизации состояния после удара </summary>
    public struct SynchronizationInfo
    {
        public string MatchId { get; set; }
        public RulesShotResult RulesShotResult { get; set; }
    }
}