using BilliardServer.Core.Common;
using Kborod.SharedDto;
using MediatR;

namespace BilliardServer.Application.Features.Users
{
    public record GetUserProfilesCommand(List<string> ids) : IRequest<Result<List<UserProfileDto>>>;
}
