using BilliardServer.Core.Common;
using MediatR;

namespace BilliardServer.Application.Features.Users
{
    public record UserHearbeatEvent(string UserId) : INotification;
}
