using BilliardServer.Core.Common;
using Kborod.BilliardCore.Enums;
using MediatR;

namespace BilliardServer.Application.Features.MatchMaking
{
    public record DeleteMatchCommand(string MatchId) : IRequest;
}
