namespace BilliardServer.Application.Matches
{
    public static class RatingConfig
    {
        /// <summary> Количество партий, в которых  игрок считается новичком </summary>
        private const int RATE_KOEF_BEGINNER_PARTY_COUNT = 30;

        /// <summary> Граница очков, после которых уменьшается коэффициент для рейтинга </summary>
        private const int RATE_KOEF_MASTERS_BORDER_POINTS = 25000;

        /// <summary> Коэффициент рейтинга для новичков </summary>
        private const int RATE_KOEF_BEGINNER = 400;

        /// <summary> Коэффициент рейтинга для игроков любителей (рейт меньше RATE_KOEF_MASTERS_BORDER_POINTS), исключая новичков </summary>
        private const int RATE_KOEF_AMATEUR = 200;

        /// <summary> Коэффициент рейтинга для мастеров (рейт выше RATE_KOEF_MASTERS_BORDER_POINTS) </summary>
        private const int RATE_KOEF_MASTER = 100;

        public static int GetRatePointsByParams(int userRate, int oppRate, int userPartiesCount, int userWin)
        {
            int delta = userRate - oppRate;

            var expectedResult = (double)delta / 20000;
            expectedResult = 0.5 + 0.5 * expectedResult;
            expectedResult = Math.Clamp(expectedResult, 0, 1);

            int k;
            if (userPartiesCount <= RATE_KOEF_BEGINNER_PARTY_COUNT) k = RATE_KOEF_BEGINNER;
            else if (userRate <= RATE_KOEF_MASTERS_BORDER_POINTS) k = RATE_KOEF_AMATEUR;
            else k = RATE_KOEF_MASTER;

            int res = (int)((double)k * ((double)userWin - expectedResult));
            if (res == 0) { res = (userWin == 1) ? 1 : -1; }
            return res;
        }
    }
}
