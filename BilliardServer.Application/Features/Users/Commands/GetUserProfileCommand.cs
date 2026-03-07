using BilliardServer.Core.Common;
using Kborod.SharedDto;
using MediatR;

namespace BilliardServer.Application.Features.Users
{
    public record GetUserProfileCommand(string id) : IRequest<Result<UserProfileDto>>;
}
