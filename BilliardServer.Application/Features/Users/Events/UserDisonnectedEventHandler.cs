using BilliardServer.Application.Abstractions;
using BilliardServer.Application.Matches;
using BilliardServer.Application.MatchMaking;
using MediatR;

namespace BilliardServer.Application.Features.Users
{
    public class UserDisonnectedEventHandler : INotificationHandler<UserDisconnectedEvent>
    {
        private IUserDisconnectedHandler _handler;
        private MatchMakingService _matchMakingService;
        private MatchesService _matchesService;
        private RematchesService _rematchesService;

        public UserDisonnectedEventHandler(IUserDisconnectedHandler handler, 
            MatchMakingService matchMakingService, MatchesService matchesService, RematchesService rematchesService)
        {
            _handler = handler;
            _matchMakingService = matchMakingService;
            _matchesService = matchesService;
            _rematchesService = rematchesService;
        }

        public async Task Handle(UserDisconnectedEvent notification, CancellationToken cancellationToken)
        {
            await _handler.UserDisconnectedHandler(notification.UserId, notification.BeforeStartNewSession);
            await _matchMakingService.CancelSearch(notification.UserId);
            await _matchesService.UserDisconnected(notification.UserId);
            await _rematchesService.UserDisconnected(notification.UserId);
        }
    }
}
