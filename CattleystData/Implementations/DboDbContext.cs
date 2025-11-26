using CattleystData.Interfaces;
using CattleystData.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CattleystData.Implementations
{
    public class DboDbContext : IDisposable, IDboDbReadContext, IDboDbWriteContext
    {
        private readonly SqlConnection _conn;

        public DboDbContext(string connectionString)
        {
            _conn = new SqlConnection(connectionString);
            _conn.Open();
        }

        public void Dispose()
        {
            _conn.Dispose();
        }

        public Task<IEnumerable<Location>> LocationList()
        {
            string sql = @"
                select
                    LocationId
                    ,LocationName
                from
                    [dbo].[Location]
            ;";
            return _conn.QueryAsync<Location>(sql, commandType: CommandType.Text);
        }
    }
}
