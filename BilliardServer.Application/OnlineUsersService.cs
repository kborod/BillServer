using BilliardServer.Core.Abstractions;
using BilliardServer.Core.Common;
using BilliardServer.Core.Models;

namespace Billiard.Application
{
    //TODO BORODIN Добавить реализацию
    /// <summary>
    /// В случае, если на клиенте нет сообщений для отправки на сервер, клиент каждые 5 секунд отправляет IamAlive signalr запрос.
    /// Этот сервис хранит информацию о залогиненных юзерах (memory cach объекты?)
    /// </summary>
    public class OnlineUsersService
    {
        public event Action<long>? UserConnected;
        public event Action<long>? UserDisconnected;
    }
}
