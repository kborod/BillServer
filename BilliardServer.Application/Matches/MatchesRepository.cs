

using System.Collections.Concurrent;

namespace BilliardServer.Application.Matches
{
    public class MatchesRepository
    {
        private ConcurrentDictionary<Guid, Match> _matches = new();

        public void CreateMatch(long player1, long player2)
        {
            var id = Guid.NewGuid();
            var match = new Match(id, player1, player2);
            _matches.TryAdd(id, match);
        }
    }
}
