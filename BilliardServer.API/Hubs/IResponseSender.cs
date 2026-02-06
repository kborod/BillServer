using BilliardServer.Core.Dto.Hub;

namespace BilliardServer.API.Hubs
{
    public interface IResponseSender
    {
        Task Send(ResponseEnvelope responseEnvelope);
    }
}
