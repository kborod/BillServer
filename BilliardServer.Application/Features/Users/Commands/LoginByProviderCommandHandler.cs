using BilliardServer.Core.Abstractions;
using BilliardServer.Core.Common;
using MediatR;

namespace BilliardServer.Application.Features.Users
{
    public class LoginByProviderCommandHandler : IRequestHandler<LoginByProviderCommand, AuthByProviderResult>
    {
        private readonly IAuthService _authService;

        public LoginByProviderCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<AuthByProviderResult> Handle(LoginByProviderCommand request, CancellationToken cancellationToken)
        {
            return await _authService.LoginByProvider(request.provider, request.providerKey, request.name, request.email);
        }
    }
}
