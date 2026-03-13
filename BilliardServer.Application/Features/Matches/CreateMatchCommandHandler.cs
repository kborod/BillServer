using BilliardServer.Application.Matches;
using BilliardServer.Core.Common;
using MediatR;

namespace BilliardServer.Application.Features.Matches
{
    public class CreateMatchCommandHandler : IRequestHandler<CreateMatchCommand, Result>
    {
        private readonly MatchesService _matchesService;

        public CreateMatchCommandHandler(MatchesService matchesService)
        {
            _matchesService = matchesService;
        }

        public Task<Result> Handle(CreateMatchCommand request, CancellationToken cancellationToken)
        {
            return _matchesService.CreateMatch(request.User1Id, request.User2Id, request.turningPlayer, request.gameType, request.betType);
        }
    }
}
