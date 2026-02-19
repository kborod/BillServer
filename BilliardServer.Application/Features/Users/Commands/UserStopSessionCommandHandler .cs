using BilliardServer.Application.Abstractions;
using BilliardServer.Core.Common;
using MediatR;

namespace BilliardServer.Application.Features.Users
{
    public class UserStopSessionCommandHandler : IRequestHandler<UserStopSessionCommand, Result>
    {
        private IOnlineUsersService _onlineUsersService;

        public UserStopSessionCommandHandler(IOnlineUsersService onlineUsersService)
        {
            _onlineUsersService = onlineUsersService;
        }

        public async Task<Result> Handle(UserStopSessionCommand notification, CancellationToken cancellationToken)
        {
            return await _onlineUsersService.DisconnectUser(notification.UserId);
        }
    }
}
