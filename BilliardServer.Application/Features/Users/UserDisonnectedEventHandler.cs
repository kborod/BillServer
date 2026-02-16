using BilliardServer.Core.Abstractions;
using MediatR;

namespace BilliardServer.Application.Features.Users
{
    public class UserDisonnectedEventHandler : INotificationHandler<UserDisconnectedEvent>
    {
        private IAsyncMessagingService _messagingService;

        public UserDisonnectedEventHandler(IAsyncMessagingService messagingService)
        {
            _messagingService = messagingService;
        }

        public async Task Handle(UserDisconnectedEvent notification, CancellationToken cancellationToken)
        {
            await _messagingService.UserDisconnectedHandler(notification.UserId);
        }
    }
}
