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

        public Task<Location> LocationGet(int locationId)
        {
            string sql = @"
                select
                    LocationId
                    ,LocationName
                from
                    [dbo].[Location]
                where
                    LocationId = @LocationId
            ;";
            var values = new { locationId };
            return _conn.QueryFirstOrDefaultAsync<Location>(sql, values, commandType: CommandType.Text);
        }

        public Task LocationAdd(string locationName)
        {
            string sql = @"
                insert
                    [dbo].[Location]
                (
                    LocationName
                )
                values
                    (@LocationName)
            ;";
            var values = new { locationName };
            return _conn.ExecuteAsync(sql, values, commandType: CommandType.Text);
        }

        public Task LocationUpdate(int locationId, string locationName)
        {
            string sql = @"
                update
                    [dbo].[Location]
                set
                    LocationName = @LocationName
                where
                    LocationId = @LocationId
            ;";
            var values = new { locationId, locationName };
            return _conn.ExecuteAsync(sql, values, commandType: CommandType.Text);
        }

        public Task LocationDelete(int locationId)
        {
            string sql = @"
                delete
                    [dbo].[Location]
                where
                    LocationId = @LocationId
            ;";
            var values = new { locationId };
            return _conn.ExecuteAsync(sql, values, commandType: CommandType.Text);
        }
    }
}
