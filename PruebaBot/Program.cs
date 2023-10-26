using PruebaBot.BtServicios;
using PruebaBot.BtServicios.BotHandler;
using Serilog;
using Serilog.Extensions.Hosting;
using Serilog.Sinks.SystemConsole;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


#region configuraciones de bot
var host = Host.CreateDefaultBuilder(args);



host.UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .WriteTo.Console());


host.ConfigureServices(ConfigureServices);

using var appp = host.Build();

#endregion

static void ConfigureServices(HostBuilderContext Context, IServiceCollection Services)
{
    var Configuration = Context.Configuration;
    var HostingEnvironment = Context.HostingEnvironment;

    #region base de datos
    var connectionString = Configuration.GetConnectionString("DB");
    #endregion


    #region robot
    var token = Configuration.GetValue<string>("BotConfig:Token");
    var baseUrl = Configuration.GetValue<string>("BotConfig:Proxy");
    var useProxy = Configuration.GetValue<string>("BotConfig:UseProxy");
    TelegramBotClient botClient = new TelegramBotClient(new TelegramBotClientOptions(token, baseUrl));

    var me = botClient.GetMeAsync().GetAwaiter().GetResult();

    Console.WriteLine("funciona? : [" + me.FirstName + "]");


    UpdateHandlers.BotUserName = me.Username;

    botClient.SetMyCommandsAsync(new BotCommand[]
    {
        new BotCommand(){Command = "ticket", Description="Tickets pendientes."},
        new BotCommand(){Command = "nuevo", Description="Creación de ticket."}
    }).GetAwaiter().GetResult();

    Log.Logger.Information("¡El robot de Telegram está en línea! ID del robot: {Id "+me.Id+"} ({username}), Nombre del robot: {FirstName}.", me.Id, $"@{me.Username}", me.FirstName);
    Console.WriteLine("¡El robot de Telegram está en línea! ID del robot: {Id "+me.Id+"} ({username}), Nombre del robot: {FirstName}: "+ me.Id +" @{me.Username}: "+ me.FirstName);

    var AdminUserId = Configuration.GetValue<long>("BotConfig:AdminUserId");
    //if (AdminUserId > 0)
    //{
    //    botClient.SendTextMessageAsync(2074000530, $"¡Tu robot <a href=\"tg://user?id={me.Id}\">{me.FirstName}</a> está en línea!", (int?)Telegram.Bot.Types.Enums.ParseMode.Html);
    //}

    //botClient.SendTextMessageAsync(2074000530, "Hola bienvenido").ConfigureAwait(false);
    Console.WriteLine(botClient.SendTextMessageAsync(2074000530, "Hola bienvenido").ConfigureAwait(true));

    Services.AddSingleton<ITelegramBotClient>(botClient);
    Services.AddHostedService<BotService>();
    Services.AddHostedService<UpdateRateService>();

    //BotService inicio = new BotService(botClient, );
    #endregion
}

//Configuración de aplicación.
builder.Configuration.AddJsonFile("appsettings.json");

//Configuracion token telegram
var bot = builder.Configuration.GetSection("BotConfig")["Token"];

//Store bot screaming status
var screaming = false;

//pre-assign menu text
const string firstMenu = "<b>Opcion 1</b>\n\nA Beautiful menu with a shiny inline button.";
const string secondMenu = "<b>Opcion 2</b>\n\nA Better menu with even more shiny inline button.";

// Pre-assign button text
const string nextButton = "Next";
const string backButton = "Back";
const string tutorialButton = "Tutorial";

//Build keyboards
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

app.RunAsync();
await appp.RunAsync();
