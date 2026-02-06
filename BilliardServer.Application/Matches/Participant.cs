namespace BilliardServer.Application.Matches
{
    public class Participant
    {
        public long Id { get; private set; }

        public Participant(long id)
        {
            Id = id;
        }
    }
}
