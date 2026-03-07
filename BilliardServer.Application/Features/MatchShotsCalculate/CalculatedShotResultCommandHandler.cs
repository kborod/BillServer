using BilliardServer.Application.Matches;
using BilliardServer.Core.Common;
using MediatR;

namespace BilliardServer.Application.Features.MatchShotsCalculate
{
    public class CalculatedShotResultCommandHandler : IRequestHandler<CalculatedShotResultCommand, Result>
    {
        private readonly MatchesService _matchesService;

        public CalculatedShotResultCommandHandler(MatchesService matchesService)
        {
            _matchesService = matchesService;
        }

        public async Task<Result> Handle(CalculatedShotResultCommand request, CancellationToken cancellationToken)
        {
            var match = await _matchesService.GetMatch(request.matchId);
            if (match == null)
                return Result.Fail("Match not found");
            match.ShotCalculated(request.result);
            return Result.Ok();
        }
    }
}
