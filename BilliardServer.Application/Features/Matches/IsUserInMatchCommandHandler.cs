using BilliardServer.Application.Matches;
using BilliardServer.Core.Common;
using MediatR;

namespace BilliardServer.Application.Features.MatchMaking
{
    public class IsUserInMatchCommandHandler : IRequestHandler<IsUserInMatchCommand, bool>
    {
        private readonly MatchesService _matchesService;

        public IsUserInMatchCommandHandler(MatchesService matchesService)
        {
            _matchesService = matchesService;
        }

        public Task<bool> Handle(IsUserInMatchCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_matchesService.IsUserInMatch(request.UserId));
        }
    }
}
