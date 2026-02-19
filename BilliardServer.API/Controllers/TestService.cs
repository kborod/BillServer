using BilliardServer.Application.Abstractions.AsyncMessaging;
using BilliardServer.Application.Features.Users;
using BilliardServer.Core.Dto.Hub;
using BilliardServer.Core.Dto.Hub.Responses;
using MediatR;

namespace BuilliardServer.Test
{
    public class TestService : BackgroundService
    {
        private IMediator _mediador;
        private IMessagingResponseSenderService _responseSender;
        private ILogger _logger;

        public TestService(IMediator mediator, IMessagingResponseSenderService responseSender, ILogger logger)
        {
            _mediador = mediator;
            _responseSender = responseSender;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                var isOnline = await _mediador.Send(new IsUserOnlineCommand("1"));
                if (isOnline)
                    await _responseSender.SendResponseToUser("1", ResponseEnvelope.Create(new TestResponseDto("!!!TESTRESPONSE")), _logger);

                await Task.Delay(TimeSpan.FromSeconds(7));
            }
        }
    }
}
