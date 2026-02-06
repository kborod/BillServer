using Billiard.Application;
using BilliardServer.Core.Dto.Hub;
using BilliardServer.Core.Dto.Hub.Requests;
using BilliardServer.Core.Dto.Hub.Responses;
using BilliardServer.Core.Models;
using System.Collections.Concurrent;
using System.Text.Json;

namespace BilliardServer.API.Hubs.ReliableMessageDelivery
{
    /// <summary>
    /// SignalR не гарантирует доставку сообщений, а также их порядок. Этот сервис контролирует эти моменты.
    /// Выглядит как прослойка - все входящие и исходящие SignalR сообщения проходят через него.
    /// По каждому онлайн юзеру хранит информацию о номере последнего обработанного реквеста с клиента, список последних 
    /// отправленных ответов с сервера, которые не получены клиентом.
    /// </summary>
    public class ReliableMessageDeliveryService
    {
        private ConcurrentDictionary<long, UserMessagesInfo> _userInfos = new();

        public ReliableMessageDeliveryService(OnlineUsersService onlineUsersService)
        {
            onlineUsersService.UserDisconnected += UserDisconnectedHandler;
        }

        /// <summary>
        /// Возвращает запросы, которые должны быть исполнены после получения запроса.
        /// Если нарушается порядок сообщений, либо какие-то сообщения потеряны, может возвращаться null либо несколько сообщений сразу.
        /// Если запрос не обязательный - возвращает этот запрос.
        /// Если номер запроса соответствует ожидаемому - возвращает этот запрос.
        /// Если номер запроса не соответствует ожидаемому - возвращает null, запрос игнорируется.
        /// Если получен список потерянных запросов, то возвращается список необработанных запросов.
        /// </summary>
        public async Task<List<RequestEnvelope>?> GetUnprocessedRequests(RequestEnvelope request, long userId, IResponseSender responseSender)
        {
            if (request.IsRequired == false)
            {
                return new() { request };
            }
            
            var info = GetInfo(userId);

            if (request.RequestType == RequestType.ResendLastResponses)
            {
                await ResendLastResponses(request, info, responseSender);
                return null;
            }
            else if (request.RequestType == RequestType.ResponseReceived)
            {
                UserReceivedResponseHandler(request, info);
                return null;
            }

            List<RequestEnvelope>? requests;

            if (request.RequestType == RequestType.LastRequestsList)
                requests = request.GetPayload<LastMessagesRequestDto>()?.LastRequests;
            else
                requests = new() { request };

            if (IsOrderCorrect(requests, info) == false)
            {
                await responseSender.Send(ResponseEnvelope.Create(new ResendLastMessagesResponseDto(info.LastReceivedRequestNumber + 1)));
                return null;
            }

            requests = GetUnprocessedRequests(requests, info, responseSender);

            await responseSender.Send(ResponseEnvelope.Create(new MessageReceivedResponseDto(info.LastReceivedRequestNumber)));

            return requests;
        }

        public ResponseEnvelope PrepeareForSendToUser(ResponseEnvelope response, long userId)
        {
            var info = GetInfo(userId);

            response.SequenceNumber = info.NextResponseNumber;
            info.AddResponse(response);

            return response;
        }

        private async Task ResendLastResponses(RequestEnvelope request, UserMessagesInfo info, IResponseSender sender)
        {
            var fromNumber = request.GetPayload<ResendLastMessagesRequestDto>()!.FromNumberInclusive;
            var responses = info.GetResponsesFromNumber(fromNumber);
            await sender.Send(ResponseEnvelope.Create(new LastMessagesResponseDto(responses)));
        }

        private void UserReceivedResponseHandler(RequestEnvelope request, UserMessagesInfo info)
        {
            var payload = request.Payload.Deserialize<MessageReceivedRequestDto>();
            if (payload == null)
                return;

            info.RemoveResponsesBeforeNumber(payload.LastReceivedResponse);
        }




        private bool IsOrderCorrect(List<RequestEnvelope>? requests, UserMessagesInfo info)
        {
            if (requests == null || requests.Count == 0)
                return true;

            //Проверяем порядок номеров запросов
            for (int i = 0; i < requests.Count - 1; i++)
            {
                if (requests[i].SequenceNumber + 1 != requests[i + 1].SequenceNumber)
                    return false;
            }

            var minNumberInList = requests.Min(r => r.SequenceNumber);

            return info.LastReceivedRequestNumber + 1 >= minNumberInList;
        }

        private List<RequestEnvelope>? GetUnprocessedRequests(List<RequestEnvelope>? requestsList, UserMessagesInfo info, IResponseSender responseSender)
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

        private UserMessagesInfo GetInfo(long userId)
        {
            if (_userInfos.TryGetValue(userId, out var userMessagesInfo) == false)
            {
                userMessagesInfo = new UserMessagesInfo();
                _userInfos.TryAdd(userId, userMessagesInfo);
            }
            return userMessagesInfo;
        }

        private void UserDisconnectedHandler(long id)
        {
            RemoveUserInfo(id);
        }

        private void RemoveUserInfo(long userId)
        {
            _userInfos.TryRemove(userId, out var _);
        }
    }
}
