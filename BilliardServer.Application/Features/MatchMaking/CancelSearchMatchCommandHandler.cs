using BilliardServer.Application.MatchMaking;
using MediatR;

namespace BilliardServer.Application.Features.MatchMaking
{
    public class CancelSearchMatchCommandHandler : IRequestHandler<CancelSearchMatchCommand>
    {
        private readonly MatchMakingService _matchMakingService;

        public CancelSearchMatchCommandHandler(MatchMakingService matchMakingService)
        {
            _matchMakingService = matchMakingService;
        }

        public async Task Handle(CancelSearchMatchCommand request, CancellationToken cancellationToken)
        {
            await _matchMakingService.CancelSearch(request.UserId, withNotice: true);
        }
    }
}
