using BilliardServer.Core.Common;
using BilliardServer.Core.Dto.Hub;

namespace BilliardServer.API.Hubs.ReliableMessageDelivery
{
    public class UserMessagesInfo
    {
        public int LastReceivedRequestNumber => _lastReceivedSequenceNumber;
        public int NextResponseNumber => GetNextResponseNumber();

        private readonly object _lock = new();

        private List<ResponseEnvelope> _responsesCache = new();

        private int _lastReceivedSequenceNumber = 0;
        private int _nextResponseNumber = 0;

        public bool SetLastReceivedSequenceNumber(int currentValue, int newValue)
        {
            var old = Interlocked.Exchange(ref _lastReceivedSequenceNumber, newValue);

            return currentValue == old;
        }

        public List<ResponseEnvelope> GetResponsesFromNumber(int numberInclusive)
        {
            lock (_lock)
            {
                return _responsesCache
                    .Where(r => r.SequenceNumber >= numberInclusive)
                    .OrderBy(r => r.SequenceNumber)
                    .ToList();
            }
        }

        public void RemoveResponsesBeforeNumber(int numberInclusive)
        {
            lock (_lock)
            {
                _responsesCache.RemoveAll(r => r.SequenceNumber <= numberInclusive);
            }
        }

        public void AddResponse(ResponseEnvelope response)
        {
            lock (_lock)
            {
                _responsesCache.Add(response);
            }
        }

        private int GetNextResponseNumber()
        {
            while(true)
            {
                var current = _nextResponseNumber;
                var result = current + 1;
                var old = Interlocked.CompareExchange(ref _nextResponseNumber, result, current);
                if (old == current)
                    return result;
            }
        }
    }
}
