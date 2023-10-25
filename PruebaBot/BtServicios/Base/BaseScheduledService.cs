
using Telegram.Bot.Types;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace PruebaBot.BtServicios.Base
{
    public abstract class BaseScheduledService : IHostedService, IDisposable
    {
        private readonly Timer _timer;
        private readonly string jobName;
        private readonly TimeSpan _period;
        protected readonly ILogger Logger;

        protected BaseScheduledService(string JobName, TimeSpan period, ILogger logger)
        {
            Logger = logger;
            jobName = JobName;
            _period = period;
            _timer = new Timer(Execute, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

        }

        private void Execute(object? state = null)
        {
            Console.WriteLine("Estoy en el método Execute de la clase BaseScheduledService");
            try
            {
                ExecuteAsync().Wait();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error al ejecutar la tarea programada[{ jobName }].");
            }
            finally 
            {
                _timer.Change(_period, Timeout.InfiniteTimeSpan);
            }
        }

        protected abstract Task ExecuteAsync();

        public virtual void Dispose()
        {
            Console.WriteLine("Estoy en el método Dispose de la clase BaseScheduledService");
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Estoy en el método StartAsync() de la clase BaseScheduledService");
            Logger.LogInformation("Trabajo iniciado {JobName}", jobName);
            _timer.Change(TimeSpan.FromSeconds(3), Timeout.InfiniteTimeSpan);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Estoy en el método StopAsync() de la clase BaseScheduledService");
            Logger.LogInformation($"Stop {jobName}");
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}
