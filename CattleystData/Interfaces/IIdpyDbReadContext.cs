using CattleystData.Models.Idempotency;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace CattleystData.Interfaces
{
    public interface IIdpyDbReadContext
    {
        Task<IEnumerable<OutboxMessage>> OutboxMessageList(SqlConnection connection, DbTransaction transaction);
        Task<IdempotencyRequest> IdempotencyRequestGet(Guid requestId, SqlConnection connection = null, DbTransaction transaction = null);
    }
}
