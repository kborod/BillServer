using BilliardServer.Application.Abstractions;
using BilliardServer.Application.Features.Users;
using BilliardServer.Core.Common;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace BilliardServer.Application.OnlineUsers
{
    /// <summary>
    /// В случае, если на клиенте нет сообщений для отправки на сервер, клиент каждые 5 секунд отправляет IamAlive signalr запрос.
    /// Этот сервис хранит информацию о залогиненных юзерах (memory cach объекты?)
    /// </summary>
    public class OnlineUsersService : BackgroundService, IOnlineUsersService
    {
        private readonly TimeSpan _inactivityDisconnectAfter;
        private readonly TimeSpan _startListenHeartbeatAfter;
        private readonly TimeSpan _checkPeriod;
        private readonly ConcurrentDictionary<string, UserInfo> _onlineUsers = new ConcurrentDictionary<string, UserInfo>();

        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        
        public OnlineUsersService(IOptions<OnlineUsersServiceConfig> _config, ILogger logger, IMediator mediator)
        {
            _inactivityDisconnectAfter = TimeSpan.FromSeconds(_config.Value.InactivityDisconnectAfterSeconds);
            _startListenHeartbeatAfter = TimeSpan.FromSeconds(_config.Value.StartListenHeartbeatAfterSeconds);
            _checkPeriod = TimeSpan.FromSeconds(_config.Value.CheckUsersPeriodSeconds);

            _logger = logger;
            _mediator = mediator;
        }

        public async Task<Result> ConnectUser(string userId)
        {
            if (_onlineUsers.ContainsKey(userId))
                await ProcessDisconnectUser(userId, true);

            var added = _onlineUsers.TryAdd(userId, new UserInfo(DateTime.UtcNow, false));
            
            if (added)
            {
                _logger.LogInformation($"[OnlineUsersService] User connected. id: {userId}");
                return Result.Ok();
            }
                
            else
            {
                _logger.LogError($"[OnlineUsersService] User already online. id: {userId}");
                return Result.Fail($"User already online");
            }
        }

        public async Task<Result> DisconnectUser(string userId)
        {
            if (!_onlineUsers.ContainsKey(userId))
                return Result.Fail("User already disconnected");

            await ProcessDisconnectUser(userId, false);

            return Result.Ok();
        }

        public Task HeartbeatHandler(string userId)
        {
            _onlineUsers.AddOrUpdate(
                userId,
                _ => new UserInfo(LastSeen: DateTime.UtcNow, WaitingHeartbeat: false),
                (key, old) => old.WithNewLastMessageReceived(DateTime.UtcNow)
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

                foreach (var pair in _onlineUsers)
                {
                    var userId = pair.Key;
                    var userInfo = pair.Value;

                    if (now - userInfo.LastSeen > _inactivityDisconnectAfter)
                    {
                        toRemove.Add(userId);
                    }
                    else if (now - userInfo.LastSeen > _startListenHeartbeatAfter)
                    {
                        if (userInfo.WaitingHeartbeat == false)
                        {
                            _onlineUsers.AddOrUpdate(
                                userId,
                                _ => new UserInfo(LastSeen: DateTime.UtcNow, WaitingHeartbeat: true),
                                (key, old) => old.WithWaitingHeartbeat()
                            );
                            _ = _mediator.Send(new ListenHeartbeatCommand(userId));
                        }
                    }
                }

                foreach (string userId in toRemove)
                {
                    await ProcessDisconnectUser(userId, false);
                }

                await Task.Delay(_checkPeriod, stoppingToken);
            }
        }

        private async Task ProcessDisconnectUser(string userId, bool beforeStartNewSession)
        {
            _onlineUsers.TryRemove(userId, out _);
            await _mediator.Publish(new UserDisconnectedEvent(userId, beforeStartNewSession));
            _logger.LogInformation($"User disconnected. id: {userId}");
        }

        private record UserInfo(DateTime LastSeen, bool WaitingHeartbeat)
        {
            public UserInfo WithNewLastMessageReceived(DateTime lastSeen) =>
                this with { LastSeen = lastSeen, WaitingHeartbeat = false };

            public UserInfo WithWaitingHeartbeat() =>
                this with { WaitingHeartbeat = true };
        };
    }


}
