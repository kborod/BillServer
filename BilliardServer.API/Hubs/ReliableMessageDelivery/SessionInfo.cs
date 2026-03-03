using BilliardServer.Core.Dto.Messaging;

namespace BilliardServer.API.AsyncMessaging.ReliableMessageDelivery
{
    public class SessionInfo
    {
        public string UserId { get; private set; }

        public int LastReceivedRequestNumber => _lastReceivedSequenceNumber;

        private readonly object _lock = new();

        private List<ResponseEnvelope> _responsesCache = new();

        private int _lastReceivedSequenceNumber = 0;
        private int _lastResponseNumber = 0;

        public SessionInfo(string userId)
        {
            UserId = userId;
        }

        public int GetNextResponseNumber()
        {
            while (true)
            {
                var current = _lastResponseNumber;
                var result = current + 1;
                var old = Interlocked.CompareExchange(ref _lastResponseNumber, result, current);
                if (old == current)
                    return result;
            }
        }

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

        public override string ToString()
        {
            return $"LastRequest:{LastReceivedRequestNumber}; LastResponse:{_lastResponseNumber}; ResponsesCache: {_responsesCache.Count}";
        }
    }
}
