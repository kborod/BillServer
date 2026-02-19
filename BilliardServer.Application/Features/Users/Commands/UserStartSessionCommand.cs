using BilliardServer.Core.Common;
using MediatR;

namespace BilliardServer.Application.Features.Users
{
    public record UserStartSessionCommand(string UserId) : IRequest<Result>;
}
