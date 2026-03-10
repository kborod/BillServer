
using BilliardServer.API.AsyncMessaging.Hubs;
using BilliardServer.Application.Features.Users;
using MediatR;

namespace BilliardServer.API.Features.Users
{
    public class GetUsersWithActiveConnectionCommandHandler : IRequestHandler<GetUsersWithActiveConnectionCommand, List<string>>
    {
        public Task<List<string>> Handle(GetUsersWithActiveConnectionCommand request, CancellationToken cancellationToken)
        {
            var result = request.Users
                .Where(u => GameHub.IsUserHaveActiveConnection(u))
                .ToList();
            return Task.FromResult(result);
        }
    }
}
