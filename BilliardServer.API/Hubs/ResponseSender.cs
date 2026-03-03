using BilliardServer.Core.Dto.Messaging;
using System.Text.Json;

namespace BilliardServer.API.AsyncMessaging
{
    public class ResponseSender : IResponseSender
    {
        private readonly IResponseSender _innerClient;
        private readonly ILogger _logger;
        private readonly string _targetDescription;

        public ResponseSender(
            IResponseSender innerClient, 
            ILogger logger,
            string targetDescription)
        {
            _innerClient = innerClient;
            _logger = logger;
            _targetDescription = targetDescription;
        }

        public Task ProcessResponse(ResponseEnvelope responseEnvelope)
        {
            Log(responseEnvelope);
            return _innerClient.ProcessResponse(responseEnvelope);
        }
        private void Log(ResponseEnvelope responseEnvelope)
        {
            _logger.LogInformation(
                "[Hub]HubMsgSent: {target} -> SeqNum:{number} {response}",
                _targetDescription, responseEnvelope.SequenceNumber, JsonSerializer.Serialize(responseEnvelope));
        }
    }
}
