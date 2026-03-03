namespace BilliardServer.Application.Matches
{
    public class Match
    {
        private readonly string _id;
        private readonly Participant _player1;
        private readonly Participant _player2;

        public Match(string id, string player1, string player2)
        {
            _id = id;
            _player1 = new Participant(player1);
            _player2 = new Participant(player1);
        }

        public void PeriodicCheck()
        {
        }

        private void SendStartMatchResponse()
        {

        }
    }
}
