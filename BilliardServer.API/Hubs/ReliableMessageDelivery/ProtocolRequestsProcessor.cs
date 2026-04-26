using BilliardServer.Application.Features.Users;
using BilliardServer.Core.Common;
using BilliardServer.Core.Dto.Messaging;
using BilliardServer.Core.Dto.Messaging.Requests;
using BilliardServer.Core.Dto.Messaging.Responses;
using MediatR;
using System.Collections.Concurrent;

namespace BilliardServer.API.AsyncMessaging.ReliableMessageDelivery
{
    /// <summary>
    /// SignalR не гарантирует доставку сообщений, а также их порядок. Этот сервис контролирует эти моменты.
    /// По сути дополнительный слой - все входящие и исходящие SignalR сообщения проходят через него.
    /// По каждому онлайн юзеру хранит информацию о номере последнего обработанного реквеста с клиента, список последних 
    /// отправленных ответов с сервера, которые не получены клиентом.
    /// </summary>
    public class ProtocolRequestsProcessor
    {
        private readonly ILogger _logger;

        public ProtocolRequestsProcessor(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Возвращает запросы, которые должны быть исполнены после получения запроса.
        /// Если нарушается порядок сообщений, либо какие-то сообщения потеряны, может возвращаться null либо несколько сообщений сразу.
        /// Если запрос не обязательный - возвращает этот запрос.
        /// Если номер запроса соответствует ожидаемому - возвращает этот запрос.
        /// Если номер запроса не соответствует ожидаемому - возвращает null, запрос игнорируется.
        /// Если получен список потерянных запросов, то возвращается список необработанных запросов.
        /// </summary>
        public async Task<Result<List<RequestEnvelope>?>> GetValidatedRequests(RequestEnvelope request, SessionInfo session, IMediator mediator, IResponseSender responseSender)
        {
            List<RequestEnvelope>? requests = new() { request };

            if (request.RequestType == RequestType.ResendLastResponses)
            {
                await ResendLastResponses(request, session, responseSender);
                return Result<List<RequestEnvelope>?>.Ok(null);
            }

            if (request.RequestType == RequestType.ResponseReceived)
            {
                UserReceivedResponseHandler(request, session);
                return Result<List<RequestEnvelope>?>.Ok(null);
            }

            if (request.RequestType == RequestType.IamAlive)
            {
                await mediator.Publish(new UserHearbeatEvent(session.UserId));
                return Result<List<RequestEnvelope>?>.Ok(null);
            }

            if (request.RequestType == RequestType.LastRequestsList)
            {
                requests = request.GetPayload<LastMessagesRequestDto>()?.LastRequests;

                if (requests != null && requests.Min(r => r.SequenceNumber) > session.LastReceivedRequestNumber + 1)
                    return Result<List<RequestEnvelope>?>.Fail("Lost requests not found in client cache");
            }
            else if (!request.IsRequired)
            {
                return Result<List<RequestEnvelope>?>.Ok(requests);
            }

            if (IsOrderCorrect(requests, session) == false)
            {
                await responseSender.ProcessResponse(ResponseEnvelope.Create(new ResendLastMessagesResponseDto(session.LastReceivedRequestNumber + 1)));
                return Result<List<RequestEnvelope>?>.Ok(null);
            }

            requests = GetUnprocessedRequests(requests, session);

            await responseSender.ProcessResponse(ResponseEnvelope.Create(new MessageReceivedResponseDto(session.LastReceivedRequestNumber)));

            return Result<List<RequestEnvelope>?>.Ok(requests);
        }

        private async Task ResendLastResponses(RequestEnvelope request, SessionInfo info, IResponseSender sender)
        {
            var fromNumber = request.GetPayload<ResendLastMessagesRequestDto>()!.FromNumberInclusive;
            var responses = info.GetResponsesFromNumber(fromNumber);
            await sender.ProcessResponse(ResponseEnvelope.Create(new LastMessagesResponseDto(responses)));
        }

        private void UserReceivedResponseHandler(RequestEnvelope request, SessionInfo info)
        {
            var payload = request.GetPayload<MessageReceivedRequestDto>();
            if (payload == null)
                return;

            info.RemoveResponsesBeforeNumber(payload.LastReceivedResponse);
        }

        private bool IsOrderCorrect(List<RequestEnvelope>? requests, SessionInfo info)
        {
            if (requests == null || requests.Count == 0)
                return true;

            for (int i = 0; i < requests.Count - 1; i++)
            {
                if (requests[i].SequenceNumber + 1 != requests[i + 1].SequenceNumber)
                    return false;
            }

            var minNumberInList = requests.Min(r => r.SequenceNumber);

            return info.LastReceivedRequestNumber + 1 >= minNumberInList;
        }

        private List<RequestEnvelope>? GetUnprocessedRequests(List<RequestEnvelope>? requestsList, SessionInfo info)
        {
            if (requestsList == null || requestsList.Count == 0)
                return null;

            var maxNumberInList = requestsList.Max(r => r.SequenceNumber);
            var currentReceivedNumber = info.LastReceivedRequestNumber;
            
            if (maxNumberInList <= currentReceivedNumber)
                return null;

            while (info.SetLastReceivedSequenceNumber(currentReceivedNumber, maxNumberInList) == false)
            {
                currentReceivedNumber = info.LastReceivedRequestNumber;
                if (maxNumberInList <= currentReceivedNumber) 
                    return null;
            }

            return requestsList
                .Where(r => r.SequenceNumber > currentReceivedNumber)
                .OrderBy(r => r.SequenceNumber)
                .ToList();
        }
    }
}
