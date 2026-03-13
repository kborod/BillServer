using Kborod.BilliardCore.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Kborod.BilliardCore.Rules.PoolEight
{
    public static class PoolEightRules
    {
        private static ReadOnlyCollection<int> SolidBalls = new ReadOnlyCollection<int>(new List<int>() { 1, 2, 3, 4, 5, 6, 7 });
        private static ReadOnlyCollection<int> StripedBalls = new ReadOnlyCollection<int>(new List<int>() { 9, 10, 11, 12, 13, 14, 15 });
        private static ReadOnlyCollection<int> AllAimBalls = new ReadOnlyCollection<int>(new List<int>() { 1, 2, 3, 4, 5, 6, 7, 9, 10, 11, 12, 13, 14, 15 });
        private static ReadOnlyCollection<int> EightBalls = new ReadOnlyCollection<int>(new List<int>() { 8 });
        private static ReadOnlyCollection<int> EmptyList = new ReadOnlyCollection<int>(new List<int>());

        public static PoolEightRulesShotResult ProcessShot<T>(ShotResult shotResult, bool isFirstShot, List<T> balls, PoolBallType playerBallType) where T: IBallData
        {
            var result = new PoolEightRulesShotResult();
            result.ReturnedBalls = GetReturnedBallsAfterShot(shotResult);

            var foul = GetFoulInShot(shotResult, balls, playerBallType);
            var ballTypeSelected = GetSelectedBallTypeInShot(shotResult, isFirstShot, playerBallType);

            result.TurnTransition = IsTurnTransition(shotResult, isFirstShot, foul, ballTypeSelected != PoolBallType.None, playerBallType);
            result.Foul = foul;
            result.BallTypeSelected = ballTypeSelected;
            result.GameOver = shotResult.PocketedBalls.Contains(8);
            result.UserWin = foul == FoulType.None && shotResult.PocketedBalls.Contains(8);

            return result;
        }

        public static TurnSettings GetFirstTurnSettings<T>(List<T> balls) where T : IBallData
        {
            var result = GetTurnSettings(balls, PoolBallType.None);
            result.CanMoveBall = 0;
            result.MoveOnlyInKitchen = true;
            return result;
        }

        public static TurnSettings GetTurnSettings<T>(List<T> balls, PoolBallType playerBallType, bool afterFoul) where T : IBallData
        {
            var result = GetTurnSettings(balls, playerBallType);
            result.CanMoveBall = afterFoul ? 0 : null;
            result.MoveOnlyInKitchen = false;
            return result;
        }

        private static ReadOnlyCollection<int> GetBallsByType(PoolBallType ballType)
        {
            return ballType switch
            {
                PoolBallType.Solid => SolidBalls,
                PoolBallType.Striped => StripedBalls,
                PoolBallType.None => EmptyList,
                _ => throw new System.NotImplementedException(),
            };
        }

        private static TurnSettings GetTurnSettings<T>(List<T> balls, PoolBallType playerBallType) where T : IBallData
        {
            var result = new TurnSettings
            {
                BallsAvailableToSelectAsCueball = new List<int>() { 0 },
                BallsAvailableToAim = GetBallsAvailableToAim(balls, playerBallType).ToList()
            };
            return result;
        }


        private static List<int> GetReturnedBallsAfterShot(ShotResult shotResult)
        {
            var returnedBalls = new List<int>();
            if (shotResult.PocketedBalls.Contains(0))
                returnedBalls.Add(0);
            return returnedBalls;
        }

        private static FoulType GetFoulInShot<T>(ShotResult shotResult, List<T> balls, PoolBallType playerBallType) where T : IBallData
        {
            if (IsFatalFoul())
                return FoulType.P8_EightPocketed;
            if (shotResult.PocketedBalls.Contains(8))
                return FoulType.None;
            
            if (shotResult.PocketedBalls.Contains(0))
                return FoulType.P8_CueBallPocketed;
            if (shotResult.FirstCollisionBallNum == null)
                return FoulType.P8_NoCollision;
            if (shotResult.PocketedBalls.Count == 0 && shotResult.WallsCollisionCount == 0)
                return FoulType.P8_WallsCollisionsRequired;
            if (playerBallType != PoolBallType.None )
            {
                if (shotResult.FirstCollisionBallNum.Value.GetPoolBallType() != playerBallType)
                {
                    if (shotResult.FirstCollisionBallNum.Value != 8)
                        return FoulType.P8_FirstCollision;
                    else if(!IsAllBallsPocketed(balls, playerBallType) || shotResult.PocketedBalls.Any(bNumber => bNumber.GetPoolBallType() == playerBallType))
                        return FoulType.P8_FirstCollision;
                }
            }

            return FoulType.None;

            bool IsFatalFoul()
            {
                if (shotResult.PocketedBalls.Contains(8) == false)
                    return false;
                if (shotResult.PocketedBalls.Contains(0))
                    return true;
                if (playerBallType == PoolBallType.None)
                    return true;
                if (IsAllBallsPocketed(balls, playerBallType) == false)
                    return true;
                if (shotResult.PocketedBalls.Any(bNumber => bNumber != 8 && bNumber.GetPoolBallType() == playerBallType))
                    return true;
                if (shotResult.FirstCollisionBallNum != 8)
                    return true;
                return false;
            }
        }

        private static bool IsTurnTransition(ShotResult shotResult, bool isFirstShot, FoulType foul, bool ballTypeSelected, PoolBallType playerBallType)
        {
            if (foul != FoulType.None)
                return true;
            if (shotResult.PocketedBalls.Count == 0)
                return true;
            if (ballTypeSelected)
                return false;
            if (isFirstShot && shotResult.PocketedBalls.Count > 0)
                return false;
            if (playerBallType != PoolBallType.None)
            {
                foreach (var pocketedBall in shotResult.PocketedBalls)
                {
                    if (pocketedBall.GetPoolBallType() == playerBallType)
                        return false;
                }
            }
            return true;
        }

        private static PoolBallType GetSelectedBallTypeInShot(ShotResult shotResult, bool isFirstShot, PoolBallType playerBallType)
        {
            if (isFirstShot)
            {
                return PoolBallType.None;
            }   

            if (playerBallType == PoolBallType.None &&
                shotResult.PocketedBalls.Count > 0 &&
                shotResult.PocketedBalls.Contains(0) == false)
            {
                return shotResult.PocketedBalls[0].GetPoolBallType();
            }

            return PoolBallType.None;
        }

        private static ReadOnlyCollection<int> GetBallsAvailableToAim<T>(List<T> balls, PoolBallType playerBallType) where T : IBallData
        {
            if (playerBallType == PoolBallType.None)
                return AllAimBalls;
            if (IsAllBallsPocketed(balls, playerBallType))
                return EightBalls;

            return GetBallsByType(playerBallType);
        }

        private static bool IsAllBallsPocketed<T>(List<T> balls, PoolBallType ballType) where T : IBallData
        {
            foreach (var bNumber in GetBallsByType(ballType))
            {
                if (balls[bNumber].IsRemoved == false)
                    return false;
            }
            return true;
        }
    }
}
