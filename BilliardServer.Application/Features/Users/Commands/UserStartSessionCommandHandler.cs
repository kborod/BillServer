using BilliardServer.Application.Abstractions;
using BilliardServer.Core.Common;
using MediatR;

namespace BilliardServer.Application.Features.Users
{
    public class UserStartSessionCommandHandler : IRequestHandler<UserStartSessionCommand, Result>
    {
        private IOnlineUsersService _onlineUsersService;

        public UserStartSessionCommandHandler(IOnlineUsersService onlineUsersService)
        {
            _onlineUsersService = onlineUsersService;
        }

        public async Task<Result> Handle(UserStartSessionCommand notification, CancellationToken cancellationToken)
        {
            return await _onlineUsersService.ConnectUser(notification.UserId);
        }
    }
}
