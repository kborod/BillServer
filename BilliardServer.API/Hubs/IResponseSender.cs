using BilliardServer.Core.Dto.Messaging;

namespace BilliardServer.API.AsyncMessaging
{
    public interface IResponseSender
    {
        Task ProcessResponse(ResponseEnvelope responseEnvelope);
    }
}
