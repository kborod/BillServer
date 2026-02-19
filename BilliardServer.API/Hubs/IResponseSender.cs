using BilliardServer.Core.Dto.Hub;

namespace BilliardServer.API.AsyncMessaging
{
    public interface IResponseSender
    {
        Task ProcessResponse(ResponseEnvelope responseEnvelope);
    }
}
