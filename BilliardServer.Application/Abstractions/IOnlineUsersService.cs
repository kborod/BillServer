using BilliardServer.Core.Common;
using Microsoft.Extensions.Hosting;

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