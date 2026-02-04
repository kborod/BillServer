using BilliardServer.Core.Common;
using BilliardServer.Core.Models;
using MediatR;

namespace BilliardServer.Application.Features.Users
{
    public record RegisterByEmailCommand(string name,string email, string password) : IRequest<Result<User>>;
}
