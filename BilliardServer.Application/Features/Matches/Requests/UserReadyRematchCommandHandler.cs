using BilliardServer.Application.Features.Matches.Requests;
using BilliardServer.Application.Matches;
using BilliardServer.Core.Common;
using MediatR;

namespace BilliardServer.Application.Features.MatchMaking
{
    public class UserReadyRematchCommandHandler : IRequestHandler<UserReadyRematchCommand, Result>
    {
        private readonly RematchesService _rematchesService;

        public UserReadyRematchCommandHandler(RematchesService rematchesService)
        {
            _rematchesService = rematchesService;
        }

        public async Task<Result> Handle(UserReadyRematchCommand request, CancellationToken cancellationToken)
        {
            return await _rematchesService.UserReadyRematch(request.UserId, request.matchId);
        }
    }
}
