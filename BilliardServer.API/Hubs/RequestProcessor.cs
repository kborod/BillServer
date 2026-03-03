using BilliardServer.Application.Features.MatchMaking;
using BilliardServer.Core.Dto.Messaging;
using BilliardServer.Core.Dto.Messaging.Requests.MatchMaking;
using MediatR;
using System.Text.Json;

namespace BilliardServer.API.AsyncMessaging
{
    public static class RequestProcessor
    {
        public static async Task Process(this RequestEnvelope requestEnvelope, string userId, IMediator mediator)
        {
            if (requestEnvelope.RequestType == RequestType.SearchMatch)
            {
                var dto = requestEnvelope.Payload.Deserialize<SearchMatchDto>()!;
                await mediator.Send(new SearchMatchCommand(userId, dto.GameType, dto.BetType));
            }
            else if (requestEnvelope.RequestType == RequestType.CancelSearchMatch)
            {
                await mediator.Send(new CancelSearchMatchCommand(userId));
            }
            else
            {
                throw new InvalidOperationException($"Unsupported request type: {requestEnvelope.RequestType}");
            }
        }
        //=> requestEnvelope.RequestType switch
        //{
        //    RequestType.Test => new UserAliveEvent(userId),//new JoinMatchCommand(userId, requestEnvelope.Payload.Deserialize<JoinMatchRequestDto>()!),

        //    _ => throw new InvalidOperationException($"Unsupported request type: {requestEnvelope.RequestType}")
        //};
    }
}
