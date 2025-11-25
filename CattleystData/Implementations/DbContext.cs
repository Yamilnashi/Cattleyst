using CattleystData.Interfaces;
using Microsoft.Data.SqlClient;
using System;

namespace CattleystData.Implementations
{
    public class DbContext : IDisposable, IDbReadContext, IDbWriteContext
    {
        private readonly SqlConnection _conn;

        public DbContext(string connectionString)
        {
            _conn = new SqlConnection(connectionString);
            _conn.Open();
        }

        public void Dispose()
        {
            _conn.Dispose();
        }

        public SqlConnection GetConnection()
        {
            return _conn;
        }



    }
}
