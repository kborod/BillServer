using BilliardServer.Core.Abstractions;
using BilliardServer.Core.Dto.Hub;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace BilliardServer.API.Hubs
{
    [Authorize]
    public class GameHub : Hub<IResponseSender>
    {
        private readonly IHubRequestsHandler _requestsHandler;
        private readonly ILogger<GameHub> _logger;

        public GameHub(IHubRequestsHandler requestsHandler, ILogger<GameHub> logger)
        {
            _requestsHandler = requestsHandler;
            _logger = logger;
        }

        public async Task ProcessRequest(RequestEnvelope requestEnvelope)
        {
            var userId = GetUserId();

            await _requestsHandler.RequestReceivedFromHub(requestEnvelope, userId, Clients.Caller);
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"User connected. Id: {GetUserId()}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation($"User disconnected. Id: {GetUserId()}");
            await base.OnDisconnectedAsync(exception);
        }

        private string GetUserId()
        {
            var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogError("GetUserId failed: invalid user ID claim");
                throw new Exception("Invalid user ID");
            }
                
            return userId;
        }
    }
}
