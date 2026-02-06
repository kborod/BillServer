using BilliardServer.Core.Common;
using MediatR;

namespace BilliardServer.Application.Features.Hub
{
    public class JoinMatchCommandHandler : IRequestHandler<JoinMatchCommand, Result<bool>>
    {
        //private readonly IAuthService _authService;

        //public JoinMatchCommandHandler(IAuthService authService)
        //{
        //    _authService = authService;
        //}

        public async Task<Result<bool>> Handle(JoinMatchCommand request, CancellationToken cancellationToken)
        {
            return Result<bool>.Fail("NotImplemented");
        }
    }
}
