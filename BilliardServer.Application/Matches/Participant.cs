namespace BilliardServer.Application.Matches
{
    public class Participant
    {
        public string Id { get; private set; }

        public Participant(string id)
        {
            Id = id;
        }
    }
}
