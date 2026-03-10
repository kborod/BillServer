using BilliardServer.Application.Features.MatchShotsCalculate;
using Kborod.BilliardCore;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using System.Text.Json;

namespace BilliardServer.Application.ShotCalculating
{
    public class ShotCalculationService : BackgroundService
    {
        private readonly ShotCalculationQueue _queue;
        private readonly ObjectPool<PoolShotCalculator> _pool;
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public ShotCalculationService(ShotCalculationQueue queue, ObjectPool<PoolShotCalculator> pool, IMediator mediator, ILogger logger)
        {
            _queue = queue;
            _pool = pool;
            _mediator = mediator;
            _logger = logger;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var context = await _queue.Channel.Reader.ReadAsync(stoppingToken);

                if (context is CalculatePoolShotContext poolContext)
                {
                    var calculator = _pool.Get();
                    try
                    {
                        var result = calculator.CalculateShot(poolContext);
                        _ = _mediator.Send(new CalculatedShotResultCommand(context.MatchId, result));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Calculate error. Context:{context}", context);
                        _ = _mediator.Send(new CalculatedShotResultCommand(context.MatchId, null));
                    }
                    finally
                    {
                        _pool.Return(calculator);
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}
