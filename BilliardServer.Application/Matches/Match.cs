namespace BilliardServer.Application.Matches
{
    public class Match
    {
        private readonly Guid _id;
        private readonly Participant _player1;
        private readonly Participant _player2;

        public Match(Guid id, long player1, long player2)
        {
            _id = id;
            _player1 = new Participant(player1);
            _player2 = new Participant(player1);
        }
    }
}
