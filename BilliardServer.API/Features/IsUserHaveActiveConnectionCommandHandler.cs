
using BilliardServer.API.AsyncMessaging.Hubs;
using BilliardServer.Application.Features.Users;
using MediatR;

namespace BilliardServer.API.Features.Users
{
    public class IsUserHaveActiveConnectionCommandHandler : IRequestHandler<IsUserHaveActiveConnectionCommand, bool>
    {
        public Task<bool> Handle(IsUserHaveActiveConnectionCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(GameHub.IsUserHaveActiveConnection(request.User));
        }
    }
}
