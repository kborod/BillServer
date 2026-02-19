using BilliardServer.Core.Dto.Hub;
using Microsoft.Extensions.Logging;

namespace BilliardServer.Application.Abstractions.AsyncMessaging
{
    public interface IMessagingResponseSenderService
    {
        ///<summary> Если юзер офлайн то сообщение теряется </summary>
        Task SendResponseToUser(string userId, ResponseEnvelope responseEnvelope, ILogger? logger = null);
    }
}
