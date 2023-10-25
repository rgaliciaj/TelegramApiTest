using PruebaBot.BtServicios.Base;
using PruebaBot.BtServicios.BotHandler;
using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace PruebaBot.BtServicios
{
    public class BotService : MyBackgroundService
    {
        private readonly ITelegramBotClient _client;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        

        public BotService(
            ITelegramBotClient client,
            IConfiguration configuration,
            IServiceScopeFactory serviceScopeFactory) 
        {
            Debug.WriteLine("INGRESAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            _client = client;
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingtoken)
        {
            var receiverOptions = new ReceiverOptions()
            {
                AllowedUpdates = Array.Empty<UpdateType>(),
                ThrowPendingUpdates = true,
            };

            UpdateHandlers.configuration = _configuration;
            UpdateHandlers.serviceScopeFactory = _serviceScopeFactory;
            _client.StartReceiving(updateHandler: UpdateHandlers.HandleUpdateAsync,
                pollingErrorHandler: UpdateHandlers.PollingErrorHandler,
                receiverOptions: receiverOptions,
                cancellationToken: stoppingtoken);

            return Task.CompletedTask;
        }
    }
}
