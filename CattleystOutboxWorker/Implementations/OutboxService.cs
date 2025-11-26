using CattleystData.Interfaces;
using CattleystData.Models.Idempotency;
using CattleystOutboxWorker.Interfaces;
using Microsoft.Data.SqlClient;
using Polly;
using Polly.Retry;
using System.Data.Common;

namespace CattleystOutboxWorker.Implementations
{
    public class OutboxService : IOutboxService
    {
        private readonly ILogger<OutboxService> _logger;
        private readonly IIdpyDbReadContext _dbRead;
        private readonly IIdpyDbWriteContext _dbWrite;

        public OutboxService(ILogger<OutboxService> logger,
            IIdpyDbReadContext dbRead,
            IIdpyDbWriteContext dbWrite)
        {
            _logger = logger;
            _dbRead = dbRead;
            _dbWrite = dbWrite;
        }

        public async Task HandleOutboxMessages(SqlConnection connection, DbTransaction transaction)
        {
            IEnumerable<OutboxMessage> outboxMessages = await _dbRead.OutboxMessageList(connection, transaction);

            if (outboxMessages == null || !outboxMessages.Any())
            {
                _logger.LogInformation("No messages found that need to be processed.");
                return;
            }

            _logger.LogInformation("{count} messages found.", outboxMessages.Count());

            List<Task> tasks = [];

            foreach (OutboxMessage outboxMessage in outboxMessages)
            {
                tasks.Add(CreateInboxMessage(outboxMessage, connection, transaction));
                tasks.Add(UpdateOutboxMessage(outboxMessage, connection, transaction));
            }

            await Task.WhenAll(tasks);
        }

        #region Private
        private async Task CreateInboxMessage(OutboxMessage outboxMessage, SqlConnection connection, DbTransaction transaction)
        {
            AsyncRetryPolicy retryPolicy = Policy
                .Handle<SqlException>(ex => ex.Number is 10060 or 10053)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), 
                    onRetryAsync: async (ex, timespan, retryAttempt, ctx) =>
                {
                    _logger.LogWarning("Retry {retryAttempt} after {timespan}s for InboxAdd on error: {ex}", retryAttempt, timespan.TotalSeconds, ex);
                    await Task.CompletedTask;
                });
            Guid inboxMessageId = Guid.NewGuid();
            await retryPolicy.ExecuteAsync(() => 
                _dbWrite.InboxMessageAdd(inboxMessageId, (byte)outboxMessage.EventTypeCode, outboxMessage.Payload, connection, transaction));
        }

        private async Task UpdateOutboxMessage(OutboxMessage outboxMessage, SqlConnection connection, DbTransaction transaction)
        {
            AsyncRetryPolicy retryPolicy = Policy
                .Handle<SqlException>(ex => ex.Number is 10060 or 10053)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetryAsync: async (ex, timespan, retryAttempt, ctx) =>
                    {
                        _logger.LogWarning("Retry {retryAttempt} after {timespan}s for InboxAdd on error: {ex}", retryAttempt, timespan.TotalSeconds, ex);
                        await Task.CompletedTask;
                    });
            DateTime processedDate = DateTime.UtcNow;
            await retryPolicy.ExecuteAsync(() => _dbWrite.OutboxMessageUpdate(outboxMessage.OutboxMessageId, processedDate, connection, transaction));
        }
        #endregion
    }
}
