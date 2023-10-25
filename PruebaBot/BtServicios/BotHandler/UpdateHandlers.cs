using PruebaBot.BtServicios.Base;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;

namespace PruebaBot.BtServicios.BotHandler
{
    public static class UpdateHandlers
    {
        public static string? BotUserName = null!;
        public static IConfiguration configuration = null!;
        public static IServiceScopeFactory serviceScopeFactory = null!;

        public static long AdminUserId => configuration.GetValue<long>("BotConfig:AdminUserId");
        public static string AdminUserUrl => configuration.GetValue<string>("BotConfig:AdminUserUrl");
        public static decimal MinUSDT => configuration.GetValue("MinToken:USDT", 5m);
        public static decimal FeeRate => configuration.GetValue("FeeRate", 0.1m);
        public static decimal USDTFeeRate => configuration.GetValue("USDTFeeRate", 0.01m);


        //Manejo de errores.
        public static Task PollingErrorHandler(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error: \n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.Error.WriteLine(ErrorMessage);
            Log.Error(exception, ErrorMessage);

            return Task.CompletedTask;
        }

        //Procesa actualizaciones
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var handler = update.Type switch
            {
                UpdateType.Message => BotOnMessageReceived(botClient, update.Message!),
                _ => UnknownUpdateHandlerAsync(botClient, update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Oh no，el robot cometió un error");
                await PollingErrorHandler(botClient, exception, cancellationToken);
            }
        }

        //Recepción de mensajes
        private static Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            Log.Information($"Receive message type: {message.Type}");
            throw new NotImplementedException();
        }

        private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
        {
            throw new NotImplementedException();
        }

        
    }
}
