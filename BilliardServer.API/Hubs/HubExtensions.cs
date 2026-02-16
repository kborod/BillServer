using BilliardServer.Core.Dto.Hub;
using System.Text.Json;

namespace BilliardServer.API.Hubs
{
    public static class HubExtensions
    {
        public static T GetPayload<T>(this RequestEnvelope requestEnvelope)
        {
            return requestEnvelope.Payload.Deserialize<T>()!;
        }

        public static IResponseSender AddLogging(this IResponseSender sender, ILogger logger, string userId)
        {
            return new ResponseSender(sender, logger, $"UserId_{userId}");
        }
    }
}
