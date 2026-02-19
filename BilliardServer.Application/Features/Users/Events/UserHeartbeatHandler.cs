using BilliardServer.Application.Abstractions;
using MediatR;

namespace BilliardServer.Application.Features.Users
{
    public class UserHeartbeatHandler : INotificationHandler<UserHearbeatEvent>
    {
        private IOnlineUsersService _onlineUsersService;

        public UserHeartbeatHandler(IOnlineUsersService onlineUsersService)
        {
            _onlineUsersService = onlineUsersService;
        }

        public async Task Handle(UserHearbeatEvent notificaion, CancellationToken cancellationToken)
        {
            await _onlineUsersService.HeartbeatHandler(notificaion.UserId);
        }
    }
}
