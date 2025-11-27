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

        private async Task<T> ExecuteWithConnectionAsync<T>(Func<SqlConnection, DbTransaction, Task<T>> action,
            SqlConnection externalConnection = null, DbTransaction externalTransaction = null)
        {
            bool ownsConnection = externalConnection == null; // if no external connection, create one and open it
            SqlConnection connection = externalConnection ?? new SqlConnection(_connectionString);
            DbTransaction transaction = externalTransaction;

            try
            {
                if (ownsConnection)
                {
                    await connection.OpenAsync();
                    if (transaction == null)
                    {
                        transaction = connection.BeginTransaction();
                    }
                }

                T result = await action(connection, transaction);

                if (ownsConnection 
                    && externalTransaction == null)
                {
                    transaction?.Commit();
                }

                return result;
            } 
            catch (Exception)
            {
                if (ownsConnection && 
                    externalTransaction == null) // we rollback only if we own the transaction
                {
                    transaction?.Rollback();                    
                }
                throw;
            }
            finally
            {
                if (ownsConnection)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public Task<IdempotencyRequest> IdempotencyRequestGet(Guid requestId, SqlConnection connection = null, DbTransaction transaction = null)
        {
            string sql = @"
                select
                    RequestId
                    ,RequestStateCode
                    ,RequestHash
                    ,ResponseJson
                    ,StatusCode
                    ,SavedDate
                    ,RowVersion
                from
                    [idpy].[Request]
                where
                    RequestId = @RequestId
            ;";
            var values = new { requestId };
            return ExecuteWithConnectionAsync(async (conn, trans) =>
                await conn.QueryFirstOrDefaultAsync<IdempotencyRequest>(sql, values, commandType: CommandType.Text, transaction: trans), 
                connection, transaction);
        }

        public Task<IEnumerable<OutboxMessage>> OutboxMessageList(SqlConnection connection = null, DbTransaction transaction = null)
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
            return ExecuteWithConnectionAsync(async (conn, trans) =>
                await conn.QueryAsync<OutboxMessage>(sql, commandType: CommandType.Text, transaction: trans),
                connection, transaction);
        }

        public Task<int> IdempotencyRequestAdd(Guid requestId, byte requestStateCode, string requestHash, DateTime savedDate,
            SqlConnection connection = null, DbTransaction transaction = null)
        {
            string sql = @"
                insert 
                    [idpy].[Request]
                (
                    RequestId
                    ,RequestStateCode
                    ,RequestHash
                    ,SavedDate
                )
                values
                    (@RequestId, @RequestStateCode, @RequestHash, @SavedDate)
            ;";
            var values = new { requestId, requestStateCode, requestHash, savedDate };
            return ExecuteWithConnectionAsync(async (conn, trans) =>
                await conn.ExecuteAsync(sql, values, commandType: CommandType.Text, transaction: trans),
                connection, transaction);
        }

        public Task<int> IdempotencyRequestUpdate(Guid requestId, string responseJson, int? statusCode, byte requestStateCode,
            SqlConnection connection = null, DbTransaction transaction = null)
        {
            string sql = @"
                update
                    [idpy].[Request]
                set
                    RequestStateCode = @RequestStateCode
                    ,ResponseJson = @ResponseJson
                    ,StatusCode = @StatusCode
                where
                    RequestId = @RequestId
            ;";
            var values = new { requestId, responseJson, statusCode, requestStateCode };
            return ExecuteWithConnectionAsync(async (conn, trans) =>
                await conn.ExecuteAsync(sql, values, commandType: CommandType.Text, transaction: trans), 
                connection, transaction);
        }

        public Task<int> InboxMessageAdd(Guid inboxMessageId, byte eventTypeCode, string payload, 
            SqlConnection connection = null, DbTransaction transaction = null)
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
            return ExecuteWithConnectionAsync(async (conn, trans) =>
                await conn.ExecuteAsync(sql, values, commandType: CommandType.Text, transaction: trans), 
                connection, transaction);
        }

        public Task<int> OutboxMessageUpdate(Guid outboxMessageId, DateTime processedDate,
            SqlConnection connection = null, DbTransaction transaction = null)
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
            return ExecuteWithConnectionAsync(async (conn, trans) =>
                await conn.ExecuteAsync(sql, values, commandType: CommandType.Text, transaction: trans), 
                connection, transaction);
        }
    }
}
