using BilliardServer.Core.Abstractions;
using BilliardServer.Core.Common;
using Kborod.SharedDto;
using MediatR;

namespace BilliardServer.Application.Features.Users
{
    public class GetUserProfilesCommandHandler : IRequestHandler<GetUserProfilesCommand, Result<List<UserProfileDto>>>
    {
        private readonly IUsersService _usersService;

        public GetUserProfilesCommandHandler(IUsersService usersService)
        {
            _usersService = usersService;
        }

        public Task<Result<List<UserProfileDto>>> Handle(GetUserProfilesCommand request, CancellationToken cancellationToken)
        {
            return _usersService.GetUserProfiles(request.ids);
        }
    }
}
