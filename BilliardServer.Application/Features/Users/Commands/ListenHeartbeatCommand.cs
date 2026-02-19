using BilliardServer.Core.Common;
using MediatR;

namespace BilliardServer.Application.Features.Users
{
    public record ListenHeartbeatCommand(string UserId) : IRequest;
}
