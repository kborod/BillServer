using BilliardServer.Core.Abstractions;
using BilliardServer.Core.Common;
using MediatR;

namespace BilliardServer.Application.Features.Users
{
    public class LoginByEmailCommandHandler : IRequestHandler<LoginByEmailCommand, AuthResult>
    {
        private readonly IAuthService _authService;

        public LoginByEmailCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<AuthResult> Handle(LoginByEmailCommand request, CancellationToken cancellationToken)
        {
            return await _authService.LoginByEmail(request.email, request.password);
        }
    }
}
