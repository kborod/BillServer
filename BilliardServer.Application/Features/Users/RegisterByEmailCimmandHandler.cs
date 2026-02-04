using BilliardServer.Core.Abstractions;
using BilliardServer.Core.Common;
using BilliardServer.Core.Models;
using MediatR;

namespace BilliardServer.Application.Features.Users
{
    public class RegisterByEmailCimmandHandler : IRequestHandler<RegisterByEmailCommand, Result<User>>
    {
        private readonly IAuthService _authService;

        public RegisterByEmailCimmandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result<User>> Handle(RegisterByEmailCommand request, CancellationToken cancellationToken)
        {
            return await _authService.RegisterByEmail(request.name, request.email, request.password);
        }
    }
}
