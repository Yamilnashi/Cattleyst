using Microsoft.Data.SqlClient;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace CattleystData.Interfaces
{
    public interface IIdpyDbWriteContext
    {
        Task<int> OutboxMessageUpdate(Guid outboxMessageId, DateTime processedDate, SqlConnection connection, DbTransaction transaction);
        Task<int> InboxMessageAdd(Guid inboxMessageId, byte eventTypeCode, string payload, SqlConnection connection, DbTransaction transaction);
    }
}
