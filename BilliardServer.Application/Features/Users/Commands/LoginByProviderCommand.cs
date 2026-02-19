using BilliardServer.Core.Common;
using MediatR;

namespace BilliardServer.Application.Features.Users
{
    public record LoginByProviderCommand(string provider, string providerKey, string name, string? email) : IRequest<AuthByProviderResult>;
}
