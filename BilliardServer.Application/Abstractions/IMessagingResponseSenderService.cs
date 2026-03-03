using BilliardServer.Core.Dto.Messaging;
using Microsoft.Extensions.Logging;

namespace BilliardServer.Application.Abstractions.AsyncMessaging
{
    public interface IMessagingResponseSenderService
    {
        ///<summary> Если юзер офлайн то сообщение теряется </summary>
        Task SendResponseToUser<T>(string userId, T response, ILogger? logger = null) where T : IResponse;
    }
}
