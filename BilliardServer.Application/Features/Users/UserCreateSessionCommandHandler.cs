using BilliardServer.Core.Abstractions;
using BilliardServer.Core.Common;
using MediatR;

namespace BilliardServer.Application.Features.Users
{
    public class UserCreateSessionCommandHandler : IRequestHandler<UserCreateSessionCommand, Result>
    {
        private IOnlineUsersService _onlineUsersService;
        private IAsyncMessagingService _asyncMessagingService;

        public UserCreateSessionCommandHandler(IOnlineUsersService onlineUsersService, IAsyncMessagingService asyncMessagingService)
        {
            _onlineUsersService = onlineUsersService;
            _asyncMessagingService = asyncMessagingService;
        }

        public async Task<Result> Handle(UserCreateSessionCommand notification, CancellationToken cancellationToken)
        {
            var result = await _onlineUsersService.UserConnected(notification.UserId);

            if (!result.IsSuccess)
                return result;

            return await _asyncMessagingService.UserConnectedHandler(notification.UserId);
        }
    }
}
