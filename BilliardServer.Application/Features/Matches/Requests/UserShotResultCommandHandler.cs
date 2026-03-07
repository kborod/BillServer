using BilliardServer.Application.Features.Matches.Requests;
using BilliardServer.Application.Matches;
using BilliardServer.Core.Common;
using MediatR;

namespace BilliardServer.Application.Features.MatchMaking
{
    public class UserShotResultCommandHandler : IRequestHandler<UserShotResultCommand, Result>
    {
        private readonly MatchesService _matchesService;

        public UserShotResultCommandHandler(MatchesService matchesService)
        {
            _matchesService = matchesService;
        }

        public async Task<Result> Handle(UserShotResultCommand request, CancellationToken cancellationToken)
        {
            var match = await _matchesService.GetMatchByUser(request.UserId);
            if (match == null)
                return Result.Fail("Match not found");

            return match.ShotResultReceived(request.UserId, request.Data);
        }
    }
}
