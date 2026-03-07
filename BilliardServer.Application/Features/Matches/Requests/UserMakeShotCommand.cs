using BilliardServer.Core.Common;
using Kborod.BilliardCore;
using MediatR;

namespace BilliardServer.Application.Features.Matches.Requests
{
    public record UserMakeShotCommand(MakeShotData Data, string UserId) : IRequest<Result>;
}
