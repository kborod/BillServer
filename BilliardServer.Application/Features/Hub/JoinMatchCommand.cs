using BilliardServer.Core.Common;
using BilliardServer.Core.Dto.Hub.Requests;
using MediatR;

namespace BilliardServer.Application.Features.Hub
{
    public record JoinMatchCommand(long userId, JoinMatchRequestDto request) : IRequest<Result<bool>>;
}
