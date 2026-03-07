using BilliardServer.Application.ShotCalculating;
using BilliardServer.Core.Common;
using MediatR;

namespace BilliardServer.Application.Features.MatchShotsCalculate
{
    public class CalculateShotCommandHandler : IRequestHandler<CalculateShotCommand, Result>
    {
        private readonly ShotCalculationQueue _queue;

        public CalculateShotCommandHandler(ShotCalculationQueue queue)
        {
            _queue = queue;
        }

        public async Task<Result> Handle(CalculateShotCommand request, CancellationToken cancellationToken)
        {
            await _queue.Channel.Writer.WriteAsync(request.context);
            return Result.Ok();
        }
    }
}
