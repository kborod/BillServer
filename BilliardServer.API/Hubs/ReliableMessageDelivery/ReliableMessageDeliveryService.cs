using BilliardServer.API.AsyncMessaging.Hubs;
using BilliardServer.Application.Abstractions;
using BilliardServer.Application.Abstractions.AsyncMessaging;
using BilliardServer.Application.Features.Users;
using BilliardServer.Core.Common;
using BilliardServer.Core.Dto.Messaging;
using BilliardServer.Core.Dto.Messaging.Responses;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace BilliardServer.API.AsyncMessaging.ReliableMessageDelivery
{
    /// <summary>
    /// SignalR не гарантирует доставку сообщений, а также их порядок. Этот сервис контролирует эти моменты.
    /// По сути дополнительный слой - все входящие и исходящие SignalR сообщения проходят через него.
    /// По каждому онлайн юзеру хранит информацию о номере последнего обработанного реквеста с клиента, список последних 
    /// отправленных ответов с сервера, которые не получены клиентом.
    /// </summary>
    public class ReliableMessageDeliveryService : IMessagingRequestsHandlerService, IMessagingResponseSenderService, IUserDisconnectedHandler
    {
        private ConcurrentDictionary<string, SessionInfo> _sessions = new();
        private IMediator _mediator;
        private IHubContext<GameHub, IResponseSender> _hubContext;
        private ILogger _logger;

        private ProtocolRequestsProcessor _requestsProcessor;

        public ReliableMessageDeliveryService(IMediator mediator, IHubContext<GameHub, IResponseSender> hubContext, ILogger logger)
        {
            _mediator = mediator;
            _hubContext = hubContext;
            _logger = logger;
            _requestsProcessor = new ProtocolRequestsProcessor(_logger);
        }

        public async Task RequestReceivedFromHub(RequestEnvelope requestEnvelope, string userId, IResponseSender responseSender)
        {
            responseSender = responseSender.AddLogging(_logger, userId);

            if (requestEnvelope.RequestType == RequestType.StartSession)
            {
                var result = await _mediator.Send(new UserStartSessionCommand(userId));
                if (result.IsSuccess)
                {
                    await CreateSession(userId);
                }
                else
                {
                    _logger.LogError($"[MessageDeliveryService] Server error: Start session error: {result.Error}");
                    await responseSender.ProcessResponse(ResponseEnvelope.Create(new SessionErrorResponseDto("Server error")));
                }
                return;
            }

            var isOnline = await _mediator.Send(new IsUserOnlineCommand(userId));

            if (!isOnline)
            {
                await responseSender.ProcessResponse(ResponseEnvelope.Create(new SessionErrorResponseDto("Session not found")));
                return;
            }
                

            var session = GetSession(userId);
            if (session == null)
            {
                _logger.LogError($"[MessageDeliveryService] Server error: GetSession error");
                await responseSender.ProcessResponse(ResponseEnvelope.Create(new SessionErrorResponseDto("Server error")));
                return;
            }

            var validateResult = await _requestsProcessor.GetValidatedRequests(requestEnvelope, session, _mediator, responseSender);

            if (!validateResult.IsSuccess)
            {
                await responseSender.ProcessResponse(ResponseEnvelope.Create(new SessionErrorResponseDto(validateResult.Error!)));
                var result = await _mediator.Send(new UserStopSessionCommand(userId));
                if (!result.IsSuccess)
                {
                    _logger.LogError($"[MessageDeliveryService] Server error: Stop session error: {result.Error}");
                }  
                return;
            }

            var requests = validateResult.Value;

            if (requests == null)
                return;

            await _mediator.Publish(new UserHearbeatEvent(userId));

            foreach (var request in requests)
            {
                await request.Process(userId, _mediator, responseSender);
                _logger.LogInformation($"[MessageDeliveryService] Request {request.SequenceNumber} processed");
            }

            _logger.LogInformation($"Session: {session}");
        }

        public async Task SendResponseToUser<T>(string userId, T response, ILogger? logger = null) where T : IResponse
        {
            var responseEnvelope = ResponseEnvelope.Create(response);
            if (responseEnvelope.IsRequired)
            {
                var session = GetSession(userId);
                if (session == null)
                {
                    _logger.LogWarning($"[MessageDeliveryService] Response {responseEnvelope.ResponseType} to user {userId} not sent (no active session)"); 
                    return;
                }
                    
                responseEnvelope.SequenceNumber = session.GetNextResponseNumber();
                session.AddResponse(responseEnvelope);
            }
            await _hubContext.Clients.User(userId).AddLogging(logger ?? _logger, userId).ProcessResponse(responseEnvelope);
        }

        public Task UserDisconnectedHandler(string userId, bool beforeStartNewSession)
        {
            if (!beforeStartNewSession)
                _ = SendResponseToUser(userId, new SessionErrorResponseDto("Session closed"));
            RemoveUserInfo(userId);
            return Task.CompletedTask;
        }

        private Task<Result> CreateSession(string userId)
        {
            if (_sessions.TryGetValue(userId, out var info) == true)
                _logger.LogError("[MessageDeliveryService] UserConnectedHandler received but user in active session");

            CreateSessionInfo(userId);
            return Task.FromResult(Result.Ok());
        }

        private SessionInfo? GetSession(string userId)
        {
            if (_sessions.TryGetValue(userId, out var userMessagesInfo) == false)
            {
                return null;
            }
            return userMessagesInfo;
        }

        private SessionInfo CreateSessionInfo(string userId)
        {
            var session = new SessionInfo(userId);
            var added = _sessions.TryAdd(userId, session);
            if (added == false)
            {
                _logger.LogError("[MessageDeliveryService] MessagesInfo already exist. Overrided");
                _sessions.AddOrUpdate(
                    userId,
                    _ => session,
                    (_, _) => session
                );
            }
            return session;
        }

        private void RemoveUserInfo(string userId)
        {
            _sessions.TryRemove(userId, out var _);
        }
    }
}
