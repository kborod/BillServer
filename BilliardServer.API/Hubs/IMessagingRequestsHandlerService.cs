using BilliardServer.Core.Dto.Messaging;

namespace BilliardServer.API.AsyncMessaging
{
    public interface IMessagingRequestsHandlerService
    {
        public Task RequestReceivedFromHub(RequestEnvelope requestEnvelope, string userId, IResponseSender responseSender);
    }
}
