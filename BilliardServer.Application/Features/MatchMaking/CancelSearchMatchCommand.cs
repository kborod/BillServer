using Kborod.BilliardCore.Enums;
using MediatR;

namespace BilliardServer.Application.Features.MatchMaking
{
    public record CancelSearchMatchCommand(string UserId) : IRequest;
}
