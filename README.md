# TelegramApiTest

source consulta

https://medium.com/@meysam.navaei/how-to-build-telegram-client-using-c-net-9c1572f253b5

https://github.dev/LightCountry/CoinConvertBot/blob/a072ca0c656eb28f7085075adfbdadb12191902c/src/Telegram.CoinConvertBot/BgServices/BotService.cs#L29#L44

https://gitlab.com/Athamaxy/telegram-bot-tutorial/-/blob/main/TutorialBot.cs

https://core.telegram.org/bots/tutorial

https://core.telegram.org/bots/api#getme



https://github.dev/MajMcCloud/TelegramBotFramework/blob/284634d7d1ae0f6dd08f8e4ea9a91a196c37492d/TelegramBotBase/Base/MessageClient.cs#L119#L129


using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PruebaBot.BtServicios;
using PruebaBot.BtServicios.BotHandler;
using Serilog;
using Serilog.Extensions.Hosting;
using Serilog.Sinks.SystemConsole;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        Log.Information("Iniciando la aplicación...");

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        #region Configuraciones de bot
        var Configuration = builder.Configuration;

        var token = Configuration.GetValue<string>("BotConfig:Token");
        var baseUrl = Configuration.GetValue<string>("BotConfig:Proxy");

        TelegramBotClient botClient = new TelegramBotClient(new TelegramBotClientOptions(token, baseUrl));

        var me = botClient.GetMeAsync().GetAwaiter().GetResult();

        Console.WriteLine("funciona? : [" + me.FirstName + "]");

        UpdateHandlers.BotUserName = me.Username;

        botClient.SetMyCommandsAsync(new BotCommand[]
        {
            new BotCommand(){Command = "hola", Description="Hola bienvenido, mi nombre es: " + me.FirstName},
            new BotCommand(){Command = "ticket", Description="Debe de loggearse para poder crear un ticket."},
            new BotCommand(){Command = "list", Description="Este es su listado de tickets pendientes: null"}
        }).GetAwaiter().GetResult();

        Log.Logger.Information("¡El robot de Telegram está en línea! ID del robot: {Id} ({username}), Nombre del robot: {FirstName}.", me.Id, $"@{me.Username}", me.FirstName);

        var AdminUserId = Configuration.GetValue<long>("BotConfig:AdminUserId");
        if (AdminUserId > 0)
        {
            botClient.SendTextMessageAsync(AdminUserId, $"¡Tu robot <a href=\"tg://user?id={me.Id}\">{me.FirstName}</a> está en línea!", (int?)Telegram.Bot.Types.Enums.ParseMode.Html);
        }

        botClient.SendTextMessageAsync(AdminUserId, "Hola bienvenido").ConfigureAwait(false);

        app.Services.AddSingleton<ITelegramBotClient>(botClient);
        app.Services.AddHostedService<BotService>();
        app.Services.AddHostedService<UpdateRateService>();
        #endregion

        // Store bot screaming status
        var screaming = false;

        // Pre-assign menu text
        const string firstMenu = "<b>Opcion 1</b>\n\nA Beautiful menu with a shiny inline button.";
        const string secondMenu = "<b>Opcion 2</b>\n\nA Better menu with even more shiny inline button.";

        // Pre-assign button text
        const string nextButton = "Next";
        const string backButton = "Back";
        const string tutorialButton = "Tutorial";

        // Build keyboards
        InlineKeyboardMarkup firstMenuMarkup = new(InlineKeyboardButton.WithCallbackData(nextButton));
        InlineKeyboardMarkup secondMenuMarkup = new(
            new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData(backButton) },
                new[] { InlineKeyboardButton.WithUrl(tutorialButton, "https://core.telegram.org/bots/tutorial")}
            }
        );

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
