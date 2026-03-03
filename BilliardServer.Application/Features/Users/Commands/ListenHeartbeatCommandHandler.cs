using BilliardServer.Application.Abstractions.AsyncMessaging;
using BilliardServer.Core.Dto.Messaging.Responses;
using MediatR;

namespace BilliardServer.Application.Features.Users
{
    public class ListenHeartbeatCommandHandler : IRequestHandler<ListenHeartbeatCommand>
    {
        private readonly IMessagingResponseSenderService _responseSenderService;

        public ListenHeartbeatCommandHandler(IMessagingResponseSenderService responseSenderService)
        {
            _responseSenderService = responseSenderService;
        }

        public Task Handle(ListenHeartbeatCommand request, CancellationToken cancellationToken)
        {
            _responseSenderService.SendResponseToUser(request.UserId, new AreYouAliveResponseDto());
            return Task.CompletedTask;
        }
    }
}
