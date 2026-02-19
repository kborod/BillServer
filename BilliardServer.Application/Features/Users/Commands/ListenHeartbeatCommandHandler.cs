using BilliardServer.Application.Abstractions.AsyncMessaging;
using BilliardServer.Core.Dto.Hub;
using BilliardServer.Core.Dto.Hub.Responses;
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
            _responseSenderService.SendResponseToUser(request.UserId, ResponseEnvelope.Create(new AreYouAliveResponseDto()));
            return Task.CompletedTask;
        }
    }
}
