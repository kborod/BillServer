using BilliardServer.Application.Features.Users;
using BilliardServer.Core.Abstractions;
using BilliardServer.Core.Common;
using BilliardServer.Core.Models;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Billiard.Application.OnlineUsers
{
    /// <summary>
    /// В случае, если на клиенте нет сообщений для отправки на сервер, клиент каждые 5 секунд отправляет IamAlive signalr запрос.
    /// Этот сервис хранит информацию о залогиненных юзерах (memory cach объекты?)
    /// </summary>
    public class OnlineUsersService : BackgroundService, IOnlineUsersService
    {
        private readonly TimeSpan InactivityTimeout = TimeSpan.FromSeconds(10);
        private readonly TimeSpan CheckPeriod = TimeSpan.FromSeconds(3);

        private ILogger<OnlineUsersService> _logger;
        private IMediator _mediator;

        private ConcurrentDictionary<string, UserInfo> _onlineUsers = new ConcurrentDictionary<string, UserInfo>();
        
        public OnlineUsersService(ILogger<OnlineUsersService> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public Task<Result> UserConnected(string userId)
        {
            var added = _onlineUsers.TryAdd(userId, new UserInfo(DateTime.UtcNow));
            var result = added == true ? Result.Ok() : Result.Fail("Session already started");
            return Task.FromResult(result);
        }

        public Task HeartbeatHandler(string userId)
        {
            _onlineUsers.AddOrUpdate(
                userId,
                _ => new UserInfo(DateTime.UtcNow),
                (key, old) => old with { LastSeen = DateTime.UtcNow }
            );

            return Task.CompletedTask;
        }

        public bool IsOnline(string userId)
        {
            return _onlineUsers.ContainsKey(userId);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                var toRemove = new List<string>();

                foreach (var kv in _onlineUsers)
                {
                    if (now - kv.Value.LastSeen > InactivityTimeout)
                    {
                        toRemove.Add(kv.Key);
                    }
                }

                foreach (string userId in toRemove)
                {
                    _onlineUsers.TryRemove(userId, out _);
                    await _mediator.Send(new UserDisconnectedEvent(userId));
                    _logger.LogInformation($"User disconnected. id: {userId}");
                }

                await Task.Delay(CheckPeriod, stoppingToken);
            }
        }

        private record UserInfo(DateTime LastSeen)
        {
            public UserInfo WithNewLastMessageReceived(DateTime lastSeen) =>
                this with { LastSeen = lastSeen };
        };
    }


}
