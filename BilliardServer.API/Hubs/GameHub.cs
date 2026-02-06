using BilliardServer.API.Hubs.ReliableMessageDelivery;
using BilliardServer.Core.Dto.Hub;
using BilliardServer.Core.Dto.Hub.Requests;
using BilliardServer.Core.Dto.Hub.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;

namespace BilliardServer.API.Hubs
{
    [Authorize]
    public class GameHub : Hub<IResponseSender>
    {
        private readonly ReliableMessageDeliveryService _messagesControl;
        private readonly ISender _sender;

        public GameHub(ReliableMessageDeliveryService messagesControl, ISender sender)
        {
            _messagesControl = messagesControl;
            _sender = sender;
        }

        public async Task ProcessRequest(RequestEnvelope requestEnvelope)
        {
            var userId = GetUserId();

            var requests = await _messagesControl.GetUnprocessedRequests(requestEnvelope, userId, Clients.Caller);

            if (requests == null)
                return;

            foreach (var request in requests)
            {
                await _sender.Send(requestEnvelope.ToCommand(userId));
            }
        }

        private long GetUserId()
        {
            var userIdStr = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !long.TryParse(userIdStr, out var userId))
            {
                Debug.WriteLine("GetUserId failed: invalid user ID claim");
                throw new Exception("Invalid user ID");
            }
                
            return userId;
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.Send(ResponseEnvelope.Create(new MessageReceivedResponseDto(77)));
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Логирование причины отключения
            await base.OnDisconnectedAsync(exception);
        }
    }
}
