using BilliardServer.API.Hubs.ReliableMessageDelivery;
using BilliardServer.Application.Features.Users;
using BilliardServer.Core.Abstractions;
using BilliardServer.Core.Common;
using BilliardServer.Core.Dto.Hub;
using BilliardServer.Core.Dto.Hub.Responses;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace BilliardServer.API.Hubs
{
    /// <summary>
    /// Объединяет GameHub и ReliableMessageDeliveryService.
    /// Для отправки и получения сообщений использовать этот сервис
    /// </summary>
    public class AsyncMessagingService : IAsyncMessagingService, IHubRequestsHandler
    {
        private readonly ReliableMessageDeliveryService _messagesDeliveryService;
        private readonly IHubContext<GameHub, IResponseSender> _hubContext;
        private readonly IMediator _mediator;
        private readonly ILogger<AsyncMessagingService> _logger;

        public AsyncMessagingService(
            ReliableMessageDeliveryService messagesDeliveryService, 
            IHubContext<GameHub, IResponseSender> hubContext, 
            IMediator mediator, 
            ILogger<AsyncMessagingService> logger
            )
        {
            _messagesDeliveryService = messagesDeliveryService;
            _hubContext = hubContext;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task RequestReceivedFromHub(RequestEnvelope requestEnvelope, string userId, IResponseSender responseSender)
        {
            _logger.LogInformation($"Request received number {requestEnvelope.SequenceNumber} type {requestEnvelope.RequestType}");

            var isOnline = await _mediator.Send(new IsUserOnlineCommand(userId));

            responseSender = responseSender.AddLogging(_logger, userId);

            if (!isOnline)
            {
                if (requestEnvelope.RequestType != RequestType.StartSession)
                    await responseSender.ProcessResponse(ResponseEnvelope.Create(new ErrorResponseDto("Session not started")));
                else
                    await _mediator.Send(new UserCreateSessionCommand(userId));
                return;
            }
            
            var requests = await _messagesDeliveryService.GetRequestsForProcess(requestEnvelope, userId, responseSender);
            
            if (requests == null)
                return;

            foreach (var request in requests)
            {
                await request.Process(userId, _mediator);
                _logger.LogInformation($"Request processed number{request.SequenceNumber}");
            }
        }

        public async Task SendResponseToUser(string userId, ResponseEnvelope responseEnvelope, ILogger logger)
        {
            _messagesDeliveryService.PrepeareForSendToUser(responseEnvelope, userId);
            await _hubContext.Clients.User(userId).AddLogging(_logger, userId).ProcessResponse(responseEnvelope);
        }

        public Task<Result> UserConnectedHandler(string userId)
        {
            return _messagesDeliveryService.UserConnectedHandler(userId);
        }

        public Task UserDisconnectedHandler(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
