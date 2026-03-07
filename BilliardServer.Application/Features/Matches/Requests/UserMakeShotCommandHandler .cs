using BilliardServer.Application.Features.Matches.Requests;
using BilliardServer.Application.Matches;
using BilliardServer.Core.Common;
using MediatR;

namespace BilliardServer.Application.Features.MatchMaking
{
    public class UserMakeShotCommandHandler : IRequestHandler<UserMakeShotCommand, Result>
    {
        private readonly MatchesService _matchesService;

        public UserMakeShotCommandHandler(MatchesService matchesService)
        {
            _matchesService = matchesService;
        }

        public async Task<Result> Handle(UserMakeShotCommand request, CancellationToken cancellationToken)
        {
            var match = await _matchesService.GetMatchByUser(request.UserId);
            if (match == null)
                return Result.Fail("Match not found");

            return match.MakeShotReceived(request.UserId, request.Data);
        }
    }
}
