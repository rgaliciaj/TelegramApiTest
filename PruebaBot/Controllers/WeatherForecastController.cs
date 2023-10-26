using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace PruebaBot.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private ITelegramBotClient _botClient = new TelegramBotClient("6887134526:AAEUxRp3f7Vs9Fbd-mREOQou0IQVnJSGels");

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            Console.WriteLine("HOLA ESTOY INGRESANDO A LA CONSULTA");



            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }


        [HttpPost(Name = "ingreso")]
        public IActionResult Ingreso([FromBody] WeatherForecast request)
        {
            

            var response = new
            {
                codigo = "1234",
                mensaje = "envio mensaje",
                resultado = ""
            };

            EnviarNotificacionAlBot(response.codigo);

            return Ok(response);
        }


        private async void EnviarNotificacionAlBot(string codigo)
        {
            Console.WriteLine("Si ingreso a envio de notificacion");

            var chatId = 2074000530; // Reemplaza con el ID del chat o canal del bot
            var mensaje = "¡Nuevo ingreso detectado!";

            try
            {
                Log.Information(mensaje + ": " + codigo);

                await _botClient.SendTextMessageAsync(
                            chatId: 2074000530,
                            text: $"📩 ¡Nuevo ingreso detectado!:\n número de ticket: {codigo}",
                            parseMode: ParseMode.Html,
                            cancellationToken: default
                        );
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: ["+ex.Message+"]");
            }
        }
    }
}