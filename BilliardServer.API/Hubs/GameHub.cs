using BilliardServer.Core.Dto.Messaging;
using BilliardServer.Core.Dto.Messaging.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text.Json;

namespace BilliardServer.API.AsyncMessaging.Hubs
{
    [Authorize]
    public class GameHub : Hub<IResponseSender>
    {
        private readonly static ConcurrentDictionary<string, string> _connectedUsers = new();

        private readonly IMessagingRequestsHandlerService _requestsHandler;
        private readonly ILogger _logger;

        public GameHub(IMessagingRequestsHandlerService requestsHandler, ILogger logger)
        {
            _requestsHandler = requestsHandler;
            _logger = logger;
        }

        public async Task ProcessRequest(RequestEnvelope requestEnvelope)
        {
            //TODO BORODIN messages delivery testing
            //var random = new Random();
            //if (random.Next(0, 10) > 6) return;

            _logger.LogInformation("---------------------------------------------------------");
            var userId = GetUserId();

            if (_connectedUsers.TryGetValue(userId, out var connection) && connection != Context.ConnectionId)
            {
                await Clients.Caller
                    .AddLogging(_logger, userId)
                    .ProcessResponse(ResponseEnvelope.Create(new SessionErrorResponseDto("You are Entered from another device")));
                return;
            }

            _logger.LogInformation(
                "[GameHub] HubMsgReceived: {target} -> SeqNum:{number} {response}",
                $"UserId:{userId}", requestEnvelope.SequenceNumber, JsonSerializer.Serialize(requestEnvelope));

            await _requestsHandler.RequestReceivedFromHub(requestEnvelope, userId, Clients.Caller);
            _logger.LogInformation("---------------------------------------------------------");
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();

            if (_connectedUsers.TryGetValue(userId, out var oldConnection) && oldConnection != Context.ConnectionId)
            {
                await Clients.Client(oldConnection)
                    .AddLogging(_logger, userId)
                    .ProcessResponse(ResponseEnvelope.Create(new SessionErrorResponseDto("You are Entered from another device")));
            }

            _connectedUsers[userId] = Context.ConnectionId;
            _logger.LogInformation($"[GameHub] Hub connection opened (userId:{userId}; connectionId: {Context.ConnectionId})");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserId();

            if (_connectedUsers.TryGetValue(userId, out var connection) && connection == Context.ConnectionId)
            {
                _connectedUsers.TryRemove(userId, out _);
                _logger.LogInformation($"[GameHub] Hub connection closed (userId: {userId}; connectionId: {connection})");
            }
            else
            {
                _logger.LogInformation($"[GameHub] Old hub connection closed (userId: {userId}; connectionId: {Context.ConnectionId})");
            }

            await base.OnDisconnectedAsync(exception);
        }

        private string GetUserId()
        {
            var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogError("[GameHub] GetUserId failed: invalid user ID claim");
                throw new Exception("Invalid user ID");
            }
                
            return userId;
        }
    }
}
