namespace BilliardServer.Application.Abstractions
{
    public interface IUserDisconnectedHandler
    {
        Task UserDisconnectedHandler(string userId, bool beforeStartNewSession);
    }
}