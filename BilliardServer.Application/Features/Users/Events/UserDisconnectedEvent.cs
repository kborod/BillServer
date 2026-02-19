using MediatR;

namespace BilliardServer.Application.Features.Users
{
    public record UserDisconnectedEvent(string UserId, bool BeforeStartNewSession) : INotification;
}
