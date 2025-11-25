using CattleystData.Interfaces;
using CattleystData.Models.Idempotency;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace CattleystData.Implementations
{
    public class IdpyDbContext : IIdpyDbReadContext, IIdpyDbWriteContext
    {
        private readonly string _connectionString;

        public IdpyDbContext(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            _connectionString = connectionString;
        }

        public Task<int> InboxMessageAdd(Guid inboxMessageId, byte eventTypeCode, string payload, SqlConnection connection, DbTransaction transaction)
        {
            string sql = @"
                insert
                    [idpy].[InboxMessages]
                (
                    InboxMessageId
                    ,EventTypeCode
                    ,Payload                    
                )
                values
                    (@InboxMessageId, @EventTypeCode, @Payload)
            ;";
            var values = new { inboxMessageId, eventTypeCode, payload };
            return connection.ExecuteAsync(sql, values, commandType: CommandType.Text, transaction: transaction);
        }

        public Task<IEnumerable<OutboxMessage>> OutboxMessageList(SqlConnection connection, DbTransaction transaction)
        {
            string sql = @"
                select
                    OutboxMessageId
                    ,EventTypeCode
                    ,Payload
                    ,SavedDate
                    ,ProcessedDate
                from
                    [idpy].[OutboxMessages] with (readpast)
                where
                    ProcessedDate is null
            ;";
            return connection.QueryAsync<OutboxMessage>(sql, commandType: CommandType.Text, transaction: transaction);
        }

        public Task<int> OutboxMessageUpdate(Guid outboxMessageId, DateTime processedDate, SqlConnection connection, DbTransaction transaction)
        {
            string sql = @"
                    update
                        [idpy].[OutboxMessages]
                    set
                        ProcessedDate = @ProcessedDate
                    where
                        OutboxMessageId = @OutboxMessageId
                ;";
            var values = new { outboxMessageId, processedDate };
            return connection.ExecuteAsync(sql, values, commandType: CommandType.Text, transaction: transaction);
        }
    }
}
