using BilliardServer.Core.Abstractions;
using BilliardServer.Core.Dto.Hub;
using BilliardServer.Core.Dto.Hub.Responses;

namespace BuilliardServer.Test
{
    public class TestService : BackgroundService
    {
        private IAsyncMessagingService _asyncMessagesService;
        private ILogger<TestService> _logger;

        public TestService(IAsyncMessagingService asyncMessagesService, ILogger<TestService> logger)
        {
            _asyncMessagesService = asyncMessagesService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                try
                {
                    //Если юзер офлайн, то сообщение просто теряется
                    await _asyncMessagesService.SendResponseToUser("1", ResponseEnvelope.Create(new TestResponseDto("Тестовый запрос")), _logger);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при отправке периодического сообщения");
                }

                // Периодичность — каждые 10 секунд
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }
    }
}
