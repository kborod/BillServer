using BilliardServer.Core.Common;

namespace BilliardServer.Application.Abstractions
{
    public interface IOnlineUsersService
    {
        Task<Result> ConnectUser(string userId);
        Task<Result> DisconnectUser(string userId);
        Task HeartbeatHandler(string userId);
        bool IsOnline(string userId);
    }
}