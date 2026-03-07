using BilliardServer.Application.Matches;
using BilliardServer.Core.Common;
using MediatR;

namespace BilliardServer.Application.Features.Matches.Requests
{
    public class UserMatchInitedCommandHandler : IRequestHandler<UserMatchInitedCommand, Result>
    {
        private readonly MatchesService _matchesService;

        public UserMatchInitedCommandHandler(MatchesService matchesService)
        {
            _matchesService = matchesService;
        }

        public async Task<Result> Handle(UserMatchInitedCommand request, CancellationToken cancellationToken)
        {
            var match = await _matchesService.GetMatchByUser(request.UserId);
            if (match == null)
                return Result.Fail("Match not found");

            return match.MatchInitedReceived(request.UserId);
        }
    }
}
