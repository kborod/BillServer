using BilliardServer.Core.Common;
using BilliardServer.Core.Dto.Hub;
using Microsoft.Extensions.Logging;

namespace BilliardServer.Core.Abstractions
{
    public interface IAsyncMessagingService
    {
        public Task SendResponseToUser(string userId, ResponseEnvelope responseEnvelope, ILogger logger);

        public Task<Result> UserConnectedHandler(string userId);
        public Task UserDisconnectedHandler(string userId);
    }
}