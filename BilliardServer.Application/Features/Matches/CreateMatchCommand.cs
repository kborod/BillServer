using BilliardServer.Core.Common;
using Kborod.BilliardCore.Enums;
using MediatR;

namespace BilliardServer.Application.Features.Matches
{
    public record CreateMatchCommand(string User1Id, string User2Id, GameType gameType, BetType betType) : IRequest<Result>;
}
