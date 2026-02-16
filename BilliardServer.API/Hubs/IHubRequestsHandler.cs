using BilliardServer.Core.Dto.Hub;

namespace BilliardServer.API.Hubs
{
    public interface IHubRequestsHandler
    {
        public Task RequestReceivedFromHub(RequestEnvelope requestEnvelope, string userId, IResponseSender responseSender);
    }
}
