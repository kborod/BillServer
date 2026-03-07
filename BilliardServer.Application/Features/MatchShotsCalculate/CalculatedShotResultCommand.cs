using BilliardServer.Core.Common;
using Kborod.BilliardCore.Rules;
using MediatR;

namespace BilliardServer.Application.Features.MatchShotsCalculate
{
    public record CalculatedShotResultCommand(string matchId, ITurnResult result) : IRequest<Result>;
}
