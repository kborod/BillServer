using BilliardServer.Application.Abstractions;
using MediatR;

namespace BilliardServer.Application.Features.Users
{
    public class IsUserOnlineCommandHandler : IRequestHandler<IsUserOnlineCommand, bool>
    {
        private readonly IOnlineUsersService _onlineUsersService;

        public IsUserOnlineCommandHandler(IOnlineUsersService onlineUsersService)
        {
            _onlineUsersService = onlineUsersService;
        }

        public Task<bool> Handle(IsUserOnlineCommand request, CancellationToken cancellationToken)
        {
            var isOnline = _onlineUsersService.IsOnline(request.UserId);
            return Task.FromResult(isOnline);
        }
    }
}
