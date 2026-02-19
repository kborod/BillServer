using BilliardServer.Core.Common;
using MediatR;

namespace BilliardServer.Application.Features.Users
{
    public record LoginByEmailCommand(string email, string password) : IRequest<AuthResult>;
}
