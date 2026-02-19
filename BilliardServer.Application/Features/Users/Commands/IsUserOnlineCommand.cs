using BilliardServer.Core.Common;
using MediatR;

namespace BilliardServer.Application.Features.Users
{
    public record IsUserOnlineCommand(string UserId) : IRequest<bool>;
}
