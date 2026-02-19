using BilliardServer.Application.Abstractions;
using MediatR;

namespace BilliardServer.Application.Features.Users
{
    public class UserDisonnectedEventHandler : INotificationHandler<UserDisconnectedEvent>
    {
        private IUserDisconnectedHandler _handler;

        public UserDisonnectedEventHandler(IUserDisconnectedHandler handler)
        {
            _handler = handler;
        }

        public async Task Handle(UserDisconnectedEvent notification, CancellationToken cancellationToken)
        {
            await _handler.UserDisconnectedHandler(notification.UserId, notification.BeforeStartNewSession);
        }
    }
}
