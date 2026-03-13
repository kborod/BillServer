using BilliardServer.Application.Matches;
using BilliardServer.Core.Common;
using MediatR;

namespace BilliardServer.Application.Features.Matches
{
    public class AddMatchToReplayServiceCommandHandler : IRequestHandler<AddMatchToReplayServiceCommand, Result>
    {
        private readonly RematchesService _matcheReplayService;

        public AddMatchToReplayServiceCommandHandler(RematchesService matcheReplayService)
        {
            _matcheReplayService = matcheReplayService;
        }

        public Task<Result> Handle(AddMatchToReplayServiceCommand request, CancellationToken cancellationToken)
        {
            return _matcheReplayService.AddFinishedMatch(request.context);
        }
    }
}
