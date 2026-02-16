using BilliardServer.Core.Common;
using MediatR;

namespace BilliardServer.Application.Features.Users
{
    public record UserCreateSessionCommand(string UserId) : IRequest<Result>;
}
