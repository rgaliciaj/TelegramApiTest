namespace PruebaBot.BtServicios.Base
{
    public abstract class MyBackgroundService : IHostedService, IDisposable
    {
        private Task? _excutingTask;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

        protected abstract Task ExecuteAsync(CancellationToken stoppingtoken);
        public virtual void Dispose()
        {
            _stoppingCts.Cancel();
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            _excutingTask = ExecuteAsync(_stoppingCts.Token);

            if(_excutingTask.IsCompleted) 
            {
                return _excutingTask;
            }
            return Task.CompletedTask;
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            if(_excutingTask != null) 
            {
                return;
            }

            try
            {
                _stoppingCts.Cancel();
            }
            finally
            {
                await Task.WhenAny(_excutingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }
    }
}
