using BilliardServer.Core.Common;
using MediatR;

namespace BilliardServer.Application.Features.Matches.Requests
{
    public record UserLeaveMatchCommand(string matchId, string UserId) : IRequest<Result>;
}
