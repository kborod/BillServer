using MediatR;

namespace BilliardServer.Application.Features.Users
{
    public record GetUsersWithActiveConnectionCommand(ICollection<string> Users) : IRequest<List<string>>;
}
