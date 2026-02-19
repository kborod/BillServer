using BilliardServer.Core.Dto.Hub;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Text.Json;

namespace BilliardServer.API.AsyncMessaging.Hubs
{
    [Authorize]
    public class GameHub : Hub<IResponseSender>
    {
        private readonly IMessagingRequestsHandlerService _requestsHandler;
        private readonly ILogger _logger;

        public GameHub(IMessagingRequestsHandlerService requestsHandler, ILogger logger)
        {
            _requestsHandler = requestsHandler;
            _logger = logger;
        }

        public async Task ProcessRequest(RequestEnvelope requestEnvelope)
        {
            var random = new Random();
            if (random.Next(0, 10) > 6)
            {
                return;
            }
            _logger.LogInformation("---------------------------------------------------------");
            var userId = GetUserId();
            _logger.LogInformation(
                "[Hub]HubMsgReceived: {target} -> SeqNum:{number} {response}",
                $"UserId:{userId}", requestEnvelope.SequenceNumber, JsonSerializer.Serialize(requestEnvelope));

            await _requestsHandler.RequestReceivedFromHub(requestEnvelope, userId, Clients.Caller);
            _logger.LogInformation("---------------------------------------------------------");
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"Hub opened (userId:{GetUserId()})");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation($"Hub closed (userId: {GetUserId()})");
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
