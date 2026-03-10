using BilliardServer.Application.Features.MatchMaking;
using BilliardServer.Application.Matches;
using MediatR;

namespace BilliardServer.Application.Features.Matches
{
    public class DeleteMatchCommandHandler : IRequestHandler<DeleteMatchCommand>
    {
        private readonly MatchesService _matchesService;

        public DeleteMatchCommandHandler(MatchesService matchesService)
        {
            _matchesService = matchesService;
        }

        public Task Handle(DeleteMatchCommand request, CancellationToken cancellationToken)
        {
            _matchesService.DeleteMatch(request.MatchId);
            return Task.CompletedTask;
        }
    }
}
