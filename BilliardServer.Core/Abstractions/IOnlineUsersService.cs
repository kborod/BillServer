using BilliardServer.Core.Common;
using BilliardServer.Core.Models;

namespace BilliardServer.Core.Abstractions
{
    public interface IOnlineUsersService
    {
        Task<Result> UserConnected(string userId);
        Task HeartbeatHandler(string userId);
        bool IsOnline(string userId);
    }
}