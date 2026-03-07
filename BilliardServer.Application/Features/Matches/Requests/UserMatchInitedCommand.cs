using BilliardServer.Core.Common;
using Kborod.BilliardCore.Enums;
using MediatR;

namespace BilliardServer.Application.Features.Matches
{
    public record UserMatchInitedCommand(string MatchId, string UserId) : IRequest<Result>;
}
