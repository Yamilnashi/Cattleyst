using CattleystData.Interfaces;
using CattleystData.Models.Idempotency;
using CattleystOutboxWorker.Interfaces;
using Microsoft.Data.SqlClient;
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
            Guid inboxMessageId = Guid.NewGuid();
            await _dbWrite.InboxMessageAdd(inboxMessageId, (byte)outboxMessage.EventTypeCode, outboxMessage.Payload, connection, transaction);
        }

        private async Task UpdateOutboxMessage(OutboxMessage outboxMessage, SqlConnection connection, DbTransaction transaction)
        {
            DateTime processedDate = DateTime.UtcNow;
            await _dbWrite.OutboxMessageUpdate(outboxMessage.OutboxMessageId, processedDate, connection, transaction);
        }
        #endregion
    }
}
