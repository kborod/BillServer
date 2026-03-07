using BilliardServer.Core.Abstractions;
using BilliardServer.Core.Common;
using Kborod.SharedDto;
using MediatR;

namespace BilliardServer.Application.Features.Users
{
    public class GetUserProfileCommandHandler : IRequestHandler<GetUserProfileCommand, Result<UserProfileDto>>
    {
        private readonly IUsersService _usersService;

        public GetUserProfileCommandHandler(IUsersService usersService)
        {
            _usersService = usersService;
        }

        public Task<Result<UserProfileDto>> Handle(GetUserProfileCommand request, CancellationToken cancellationToken)
        {
            return _usersService.GetUserProfile(request.id);
        }
    }
}
