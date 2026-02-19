using BilliardServer.Core.Common;
using MediatR;

namespace BilliardServer.Application.Features.Users
{
    public record UserStopSessionCommand(string UserId) : IRequest<Result>;
}
