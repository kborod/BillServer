using BilliardServer.Application.Abstractions;
using BilliardServer.Application.MatchMaking;
using MediatR;

namespace BilliardServer.Application.Features.Users
{
    public class UserDisonnectedEventHandler : INotificationHandler<UserDisconnectedEvent>
    {
        private IUserDisconnectedHandler _handler;
        private MatchMakingService _matchMakingService;

        public UserDisonnectedEventHandler(IUserDisconnectedHandler handler, MatchMakingService matchMakingService)
        {
            _handler = handler;
            _matchMakingService = matchMakingService;
        }

        public async Task Handle(UserDisconnectedEvent notification, CancellationToken cancellationToken)
        {
            await _handler.UserDisconnectedHandler(notification.UserId, notification.BeforeStartNewSession);
            await _matchMakingService.CancelSearch(notification.UserId);
        }
    }
}
