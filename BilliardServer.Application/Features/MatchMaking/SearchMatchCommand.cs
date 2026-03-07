using BilliardServer.Core.Common;
using Kborod.BilliardCore.Enums;
using MediatR;

namespace BilliardServer.Application.Features.MatchMaking
{
    public record SearchMatchCommand(string UserId, GameType gameType, BetType betType) : IRequest<Result>;
}
