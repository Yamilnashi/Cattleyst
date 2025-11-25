using CattleystData.Interfaces;
using CattleystOutboxWorker.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace CattleystOutboxWorker
{
    public class OutboxMessagesWorker : BackgroundService
    {
        private readonly ILogger<OutboxMessagesWorker> _logger;
        private readonly IOutboxService _outboxRetrieval;
        private readonly IConfiguration _config;

        public OutboxMessagesWorker(ILogger<OutboxMessagesWorker> logger,
            IOutboxService outboxRetrieval,
            IConfiguration config)
        {
            _logger = logger;
            _outboxRetrieval = outboxRetrieval;
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                string? connectionString = _config.GetConnectionString("dbCattleyst");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new ArgumentNullException(nameof(connectionString));
                }
                
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (DbTransaction transaction = await conn.BeginTransactionAsync(stoppingToken))
                    {
                        try
                        {
                            await _outboxRetrieval.HandleOutboxMessages(conn, transaction);
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("Error handling outbox messages: {error}", ex.Message);
                            transaction.Rollback();
                        }                        
                    }
                }                

                await Task.Delay(5000, stoppingToken); // delay for 5 secs
            }
        }
    }
}
