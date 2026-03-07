using Kborod.BilliardCore.Enums;

namespace Kborod.BilliardCore
{
    public interface ICalculateContext
    {
        public string MatchId { get; }
        public GameType GameType { get; }
    }
}
