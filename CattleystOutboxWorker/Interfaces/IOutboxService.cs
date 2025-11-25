using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace CattleystOutboxWorker.Interfaces
{
    public interface IOutboxService
    {
        Task HandleOutboxMessages(SqlConnection connection, DbTransaction transaction);
    }
}
