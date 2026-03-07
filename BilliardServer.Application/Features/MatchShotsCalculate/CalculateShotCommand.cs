using BilliardServer.Core.Common;
using Kborod.BilliardCore;
using MediatR;

namespace BilliardServer.Application.Features.MatchShotsCalculate
{
    public record CalculateShotCommand(ICalculateContext context) : IRequest<Result>;
}
