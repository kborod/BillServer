using BilliardServer.Application.Features.Matches;
using BilliardServer.Application.Features.Matches.Requests;
using BilliardServer.Application.Features.MatchMaking;
using BilliardServer.Core.Common;
using BilliardServer.Core.Dto.Messaging;
using BilliardServer.Core.Dto.Messaging.Requests.Match;
using BilliardServer.Core.Dto.Messaging.Requests.MatchMaking;
using BilliardServer.Core.Dto.Messaging.Responses;
using MediatR;
using System.Text.Json;

namespace BilliardServer.API.AsyncMessaging
{
    public static class RequestProcessor
    {
        public static async Task Process(this RequestEnvelope requestEnvelope, string userId, IMediator mediator, IResponseSender responseSender)
        {
            if (requestEnvelope.RequestType == RequestType.SearchMatch)
            {
                var dto = requestEnvelope.Payload.Deserialize<SearchMatchDto>()!;
                var result = await mediator.Send(new SearchMatchCommand(userId, dto.GameType, dto.BetType));
                SendErrorIfNeed(result, responseSender);
            }
            else if (requestEnvelope.RequestType == RequestType.CancelSearchMatch)
            {
                await mediator.Send(new CancelSearchMatchCommand(userId));
            }
            else if (requestEnvelope.RequestType == RequestType.MatchInited)
            {
                var dto = requestEnvelope.Payload.Deserialize<MatchInitedDto>()!;
                var result = await mediator.Send(new UserMatchInitedCommand(dto.MatchId, userId));
                SendErrorIfNeed(result, responseSender);
            }
            else if (requestEnvelope.RequestType == RequestType.AimInfo)
            {
                var dto = requestEnvelope.Payload.Deserialize<AimInfoDto>()!;
                var result = await mediator.Send(new UserAimInfoCommand(dto.AimInfoData, userId));
                SendErrorIfNeed(result, responseSender);
            }
            else if (requestEnvelope.RequestType == RequestType.MakeShot)
            {
                var dto = requestEnvelope.Payload.Deserialize<MakeShotDto>()!;
                var result = await mediator.Send(new UserMakeShotCommand(dto.MakeShotData, userId));
                SendErrorIfNeed(result, responseSender);
            }
            else if (requestEnvelope.RequestType == RequestType.ShotResult)
            {
                var dto = requestEnvelope.Payload.Deserialize<ShotResultDto>()!;
                var result = await mediator.Send(new UserShotResultCommand(dto.SynchronizationInfo, userId));
                SendErrorIfNeed(result, responseSender);
            }
            else if (requestEnvelope.RequestType == RequestType.LeaveMatch)
            {
                var dto = requestEnvelope.Payload.Deserialize<LeaveMatchDto>()!;
                var result = await mediator.Send(new UserLeaveMatchCommand(dto.MatchId, userId));
                SendErrorIfNeed(result, responseSender);
            }
            else
            {
                throw new InvalidOperationException($"Unsupported request type: {requestEnvelope.RequestType}");
            }
        }

        private static void SendErrorIfNeed(Result result, IResponseSender responseSender)
        {
            if (result.IsSuccess == false)
                _ = responseSender.ProcessResponse(ResponseEnvelope.Create(new ErrorResponseDto(result.Error!)));
        }
    }
}
