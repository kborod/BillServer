using MediatR;

namespace BilliardServer.Application.Features.Users
{
    public record IsUserHaveActiveConnectionCommand(string User) : IRequest<bool>;
}
