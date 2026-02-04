using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Security.Claims;

namespace BuilliardServer
{
    [Authorize]
    public class GameHub : Hub
    {
        // Словарь для хранения состояний матчей (в реальности используйте DB или Redis для масштабируемости)
        private static readonly Dictionary<string, GameState> _matches = new();

        // Подключение игрока к матчу
        public async Task JoinMatch(string matchId, string playerId)
        {
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            Debug.WriteLine($"Player {playerId} is trying to join match {matchId}");
            await Task.Yield();


            //await Groups.AddToGroupAsync(Context.ConnectionId, matchId);

            //if (!_matches.ContainsKey(matchId))
            //{
            //    _matches[matchId] = new GameState { Player1 = playerId, Turn = playerId };
            //}
            //else
            //{
            //    var state = _matches[matchId];
            //    if (state.Player2 == null)
            //    {
            //        state.Player2 = playerId;
            //    }
            //    // Уведомляем обоих игроков о старте
            //    await Clients.Group(matchId).SendAsync("MatchStarted", state);
            //}
        }

        // Отправка хода
        public async Task MakeMove(string matchId, string playerId, object moveData)
        {
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            if (_matches.TryGetValue(matchId, out var state) && state.Turn == playerId)
            {
                // Обработка хода (ваша логика игры)
                // Например, обновляем состояние
                state.LastMove = moveData;
                state.Turn = state.Player1 == playerId ? state.Player2 : state.Player1;

                // Отправляем обновление всем в группе (матче)
                await Clients.Group(matchId).SendAsync("MoveReceived", moveData, state.Turn);
            }
        }

        // Отключение (handle disconnect)
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string connectionId = Context.ConnectionId;
            Debug.WriteLine($"OnDisconnectedAsync {connectionId}");
            // Найти матч по ConnectionId и обработать disconnect (уведомить оппонента)
            await base.OnDisconnectedAsync(exception);
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User!.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? Context.User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                Context.Abort(); // Отключаем без валидного пользователя
                Debug.WriteLine("OnConnectedAsync aborted: no valid user ID");
                return;
            }

            // Можно сохранить ConnectionId → playerId в кэше/БД
            await base.OnConnectedAsync();

            string connectionId = Context.ConnectionId;
            Debug.WriteLine($"OnConnectedAsync {connectionId}");
            await Clients.Client(connectionId).SendAsync("ReceiveMessage", $"You are connected with ID: {connectionId}. Your ID: {userId}");
            await base.OnConnectedAsync();
        }
    }

    public class GameState
    {
        public required string Player1 { get; set; }
        public required string Player2 { get; set; }
        public required string Turn { get; set; } // Чей ход
        public required object LastMove { get; set; } // Данные хода (JSON или модель)
    }
}
