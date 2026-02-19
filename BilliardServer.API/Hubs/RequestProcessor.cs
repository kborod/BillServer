using BilliardServer.Application.Features.Users;
using BilliardServer.Core.Dto.Hub;
using MediatR;

namespace BilliardServer.API.AsyncMessaging
{
    public static class RequestProcessor
    {
        public static async Task Process(this RequestEnvelope requestEnvelope, string userId, IMediator sender)
        {
            
        }
        //=> requestEnvelope.RequestType switch
        //{
        //    RequestType.Test => new UserAliveEvent(userId),//new JoinMatchCommand(userId, requestEnvelope.Payload.Deserialize<JoinMatchRequestDto>()!),

        //    _ => throw new InvalidOperationException($"Unsupported request type: {requestEnvelope.RequestType}")
        //};
    }
}
