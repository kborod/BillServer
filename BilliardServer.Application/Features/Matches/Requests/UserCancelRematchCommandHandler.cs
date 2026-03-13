using BilliardServer.Application.Features.Matches.Requests;
using BilliardServer.Application.Matches;
using BilliardServer.Core.Common;
using MediatR;

namespace BilliardServer.Application.Features.MatchMaking
{
    public class UserCancelRematchCommandHandler : IRequestHandler<UserCancelRematchCommand, Result>
    {
        private readonly RematchesService _rematchesService;

        public UserCancelRematchCommandHandler(RematchesService rematchesService)
        {
            _rematchesService = rematchesService;
        }

        public Task<Result> Handle(UserCancelRematchCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_rematchesService.UserCancelRematch(request.UserId, request.matchId));
        }
    }
}
