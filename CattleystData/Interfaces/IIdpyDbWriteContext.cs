using Microsoft.Data.SqlClient;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace CattleystData.Interfaces
{
    public interface IIdpyDbWriteContext
    {
        Task<int> OutboxMessageUpdate(Guid outboxMessageId, DateTime processedDate,
            SqlConnection connection = null, DbTransaction transaction = null);
        Task<int> InboxMessageAdd(Guid inboxMessageId, byte eventTypeCode, string payload, 
            SqlConnection connection = null, DbTransaction transaction = null);
        Task<int> IdempotencyRequestAdd(Guid requestId, byte requestStateCode, string requestHash, DateTime savedDate, 
            SqlConnection connection = null, DbTransaction transaction = null);
        Task<int> IdempotencyRequestUpdate(Guid requestId, string responseJson, int? statusCode,
            byte requestStateCode, SqlConnection connection = null, DbTransaction transaction = null);
    }
}
