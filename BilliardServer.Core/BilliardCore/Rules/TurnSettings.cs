using System.Collections.Generic;

namespace Kborod.BilliardCore.Rules
{
    public struct TurnSettings
    {
        public int? CanMoveBall { get;set; }
        public bool MoveOnlyInKitchen { get; set; }

        public List<int> BallsAvailableToAim { get; set; }
        public List<int> BallsAvailableToSelectAsCueball { get; set; }

        public bool NeedSelectPocket => false;
    }
}
