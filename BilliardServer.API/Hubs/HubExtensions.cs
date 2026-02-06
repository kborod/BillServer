using BilliardServer.Application.Features.Hub;
using BilliardServer.Core.Dto.Hub;
using BilliardServer.Core.Dto.Hub.Requests;
using MediatR;
using System.Text.Json;

namespace BilliardServer.API.Hubs
{
    public static class HubExtensions
    {
        public static IBaseRequest ToCommand(this RequestEnvelope requestEnvelope, long userId)
        => requestEnvelope.RequestType switch
        {
            RequestType.JoinMatch => new JoinMatchCommand(userId, requestEnvelope.Payload.Deserialize<JoinMatchRequestDto>()!),

            _ => throw new InvalidOperationException($"Unsupported request type: {requestEnvelope.RequestType}")
        };

        public static T GetPayload<T>(this RequestEnvelope requestEnvelope)
        {
            return requestEnvelope.Payload.Deserialize<T>();
        }
    }
}
