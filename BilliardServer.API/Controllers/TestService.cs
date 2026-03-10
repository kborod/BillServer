using BilliardServer.Application.Abstractions.AsyncMessaging;
using BilliardServer.Application.Features.MatchShotsCalculate;
using BilliardServer.Application.Features.Users;
using BilliardServer.Core.Dto.Messaging.Responses;
using Kborod.BilliardCore;
using MediatR;
using System.Text.Json;

namespace BuilliardServer.Test
{
    public class TestService : BackgroundService
    {
        private IMediator _mediator;
        private IMessagingResponseSenderService _responseSender;
        private ILogger _logger;

        public TestService(IMediator mediator, IMessagingResponseSenderService responseSender, ILogger logger)
        {
            _mediator = mediator;
            _responseSender = responseSender;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(2000);
            
            //var s = "{\"MatchId\":\"1\",\"GameType\":1,\"BallDatas\":[{\"Number\":0,\"IsRemoved\":false,\"X\":179.5,\"Y\":-43.5},{\"Number\":1,\"IsRemoved\":false,\"X\":239.3,\"Y\":-10.3},{\"Number\":2,\"IsRemoved\":true,\"X\":217,\"Y\":14},{\"Number\":3,\"IsRemoved\":true,\"X\":201.2,\"Y\":-13},{\"Number\":4,\"IsRemoved\":true,\"X\":217,\"Y\":-22},{\"Number\":5,\"IsRemoved\":true,\"X\":217,\"Y\":-40},{\"Number\":6,\"IsRemoved\":true,\"X\":201.2,\"Y\":23},{\"Number\":7,\"IsRemoved\":true,\"X\":169.7,\"Y\":5},{\"Number\":8,\"IsRemoved\":true,\"X\":185.5,\"Y\":-4},{\"Number\":9,\"IsRemoved\":true,\"X\":154,\"Y\":-4},{\"Number\":10,\"IsRemoved\":true,\"X\":201.2,\"Y\":5},{\"Number\":11,\"IsRemoved\":true,\"X\":217,\"Y\":32},{\"Number\":12,\"IsRemoved\":true,\"X\":169.7,\"Y\":-13},{\"Number\":13,\"IsRemoved\":true,\"X\":217,\"Y\":-4},{\"Number\":14,\"IsRemoved\":true,\"X\":201.2,\"Y\":-31},{\"Number\":15,\"IsRemoved\":true,\"X\":185.5,\"Y\":14}],\"AimInfo\":{\"CueBall\":0,\"DirectionX\":0.8703675,\"DirectionY\":0.4924027,\"Pocket\":null,\"CueBallX\":null,\"CueBallY\":null,\"SpinX\":0,\"SpinY\":0,\"Power\":1,\"CueId\":1,\"IsBallMovingNow\":false},\"TurningPlayerId\":\"1\",\"TurningPlayerBallType\":0,\"OppPlayerId\":\"2\",\"IsFirstSHot\":true,\"OnlyKitchen\":false,\"CuePower\":300}";
            //var context = JsonSerializer.Deserialize<CalculatePoolShotContext>(s);
            //await _mediator.Send(new CalculateShotCommand(context));

            //while (true)
            //{
            //    var isOnline = await _mediator.Send(new IsUserOnlineCommand("1"));
            //    if (isOnline)
            //        await _responseSender.SendResponseToUser("1", new TestResponseDto("!!!TESTRESPONSE"), _logger);

            //    await Task.Delay(TimeSpan.FromSeconds(7));
            //}
        }
    }
}
