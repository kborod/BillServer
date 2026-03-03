using BilliardServer.Application.MatchMaking;
using MediatR;

namespace BilliardServer.Application.Features.MatchMaking
{
    public class SearchMatchCommandHandler : IRequestHandler<SearchMatchCommand>
    {
        private readonly MatchMakingService _matchMakingService;

        public SearchMatchCommandHandler(MatchMakingService matchMakingService)
        {
            _matchMakingService = matchMakingService;
        }

        public async Task Handle(SearchMatchCommand request, CancellationToken cancellationToken)
        {
            await _matchMakingService.SearchMatch(request.UserId, request.gameType, request.betType);
        }
    }
}
