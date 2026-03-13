using BilliardServer.Core.Common;
using MediatR;

namespace BilliardServer.Application.Features.Matches.Requests
{
    public record UserReadyRematchCommand(string matchId, string UserId) : IRequest<Result>;
}
