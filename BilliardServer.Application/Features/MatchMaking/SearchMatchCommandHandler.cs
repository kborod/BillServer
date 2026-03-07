using BilliardServer.Application.MatchMaking;
using BilliardServer.Core.Common;
using MediatR;

namespace BilliardServer.Application.Features.MatchMaking
{
    public class SearchMatchCommandHandler : IRequestHandler<SearchMatchCommand, Result>
    {
        private readonly MatchMakingService _matchMakingService;

        public SearchMatchCommandHandler(MatchMakingService matchMakingService)
        {
            _matchMakingService = matchMakingService;
        }

        public Task<Result> Handle(SearchMatchCommand request, CancellationToken cancellationToken)
        {
            return _matchMakingService.SearchMatch(request.UserId, request.gameType, request.betType);
        }
    }
}
