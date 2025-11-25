namespace CattleystOutboxWorker
{
    public class OutboxMessagesWorker : BackgroundService
    {
        private readonly ILogger<OutboxMessagesWorker> _logger;

        public OutboxMessagesWorker(ILogger<OutboxMessagesWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
