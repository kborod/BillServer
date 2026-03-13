using BilliardServer.Application.Matches;
using BilliardServer.Core.Common;
using Kborod.BilliardCore.Enums;
using MediatR;

namespace BilliardServer.Application.Features.Matches
{
    public record AddMatchToReplayServiceCommand(MatchContext context) : IRequest<Result>;
}
