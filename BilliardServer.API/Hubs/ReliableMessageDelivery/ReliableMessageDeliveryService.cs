using BilliardServer.Core.Common;
using BilliardServer.Core.Dto.Hub;
using BilliardServer.Core.Dto.Hub.Requests;
using BilliardServer.Core.Dto.Hub.Responses;
using System.Collections.Concurrent;

namespace BilliardServer.API.Hubs.ReliableMessageDelivery
{
    /// <summary>
    /// SignalR не гарантирует доставку сообщений, а также их порядок. Этот сервис контролирует эти моменты.
    /// По сути дополнительный слой - все входящие и исходящие SignalR сообщения проходят через него.
    /// По каждому онлайн юзеру хранит информацию о номере последнего обработанного реквеста с клиента, список последних 
    /// отправленных ответов с сервера, которые не получены клиентом.
    /// </summary>
    public class ReliableMessageDeliveryService
    {
        private ConcurrentDictionary<string, UserMessagesInfo> _userInfos = new();
        private ILogger<ReliableMessageDeliveryService> _logger;

        public ReliableMessageDeliveryService(ILogger<ReliableMessageDeliveryService> logger)
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
        public async Task<List<RequestEnvelope>?> GetRequestsForProcess(RequestEnvelope request, string userId, IResponseSender responseSender)
        {
            var info = GetInfo(userId);

            List<RequestEnvelope>? requests = new() { request }; ;

            if (request.IsRequired == false)
            {
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
                else if (request.RequestType == RequestType.LastRequestsList)
                {
                    requests = request.GetPayload<LastMessagesRequestDto>()?.LastRequests;
                }
                else
                {
                    return requests;
                }
            }

            if (IsOrderCorrect(requests, info) == false)
            {
                await responseSender.ProcessResponse(ResponseEnvelope.Create(new ResendLastMessagesResponseDto(info.LastReceivedRequestNumber + 1)));
                return null;
            }

            requests = GetUnprocessedRequests(requests, info);

            await responseSender.ProcessResponse(ResponseEnvelope.Create(new MessageReceivedResponseDto(info.LastReceivedRequestNumber)));

            return requests;
        }

        public ResponseEnvelope PrepeareForSendToUser(ResponseEnvelope response, string userId)
        {
            if (!response.IsRequired)
                return response;

            var info = GetInfo(userId);

            response.SequenceNumber = info.NextResponseNumber;
            info.AddResponse(response);

            return response;
        }

        public Task<Result> UserConnectedHandler(string userId)
        {
            if (_userInfos.TryGetValue(userId, out var info) == true)
                _logger.LogError("UserConnectedHandler received but user in active session");

            CreateUserInfo(userId);
            return Task.FromResult(Result.Ok());
        }

        public Task UserDisconnectedHandler(string userId)
        {
            RemoveUserInfo(userId);
            return Task.CompletedTask;
        }

        private async Task ResendLastResponses(RequestEnvelope request, UserMessagesInfo info, IResponseSender sender)
        {
            var fromNumber = request.GetPayload<ResendLastMessagesRequestDto>()!.FromNumberInclusive;
            var responses = info.GetResponsesFromNumber(fromNumber);
            await sender.ProcessResponse(ResponseEnvelope.Create(new LastMessagesResponseDto(responses)));
        }

        private void UserReceivedResponseHandler(RequestEnvelope request, UserMessagesInfo info)
        {
            var payload = request.GetPayload<MessageReceivedRequestDto>();
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

        private List<RequestEnvelope>? GetUnprocessedRequests(List<RequestEnvelope>? requestsList, UserMessagesInfo info)
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

        private UserMessagesInfo GetInfo(string userId)
        {
            if (_userInfos.TryGetValue(userId, out var userMessagesInfo) == false)
            {
                _logger.LogError("MessagesInfo is not exist");
                userMessagesInfo = CreateUserInfo(userId);
            }
            return userMessagesInfo;
        }

        private UserMessagesInfo CreateUserInfo(string userId)
        {
            var userMessagesInfo = new UserMessagesInfo();
            var added = _userInfos.TryAdd(userId, userMessagesInfo);
            if (added == false)
            {
                _logger.LogError("MessagesInfo already exist. Overrided");
                _userInfos.AddOrUpdate(
                    userId,
                    _ => userMessagesInfo,
                    (_, _) => userMessagesInfo
                );
            }
            return userMessagesInfo;
        }

        private void RemoveUserInfo(string userId)
        {
            _userInfos.TryRemove(userId, out var _);
        }
    }
}
