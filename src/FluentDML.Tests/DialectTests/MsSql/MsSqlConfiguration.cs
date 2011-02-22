using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using FluentDML.Dialect;
using FluentDML.Mapping;

namespace FluentDML.Tests.DialectTests.MsSql
{
    public class MsSqlConfiguration : DialectTestFixtureConfiguration
    {
        public override IDialect GetDialect(Map map)
        {
            return new MsSqlDialect(map);
        }

        public override IDbConnection GetOpenConnection()
        {
            var connStr = ConfigurationManager.ConnectionStrings["MsSql"].ConnectionString;
            var conn = new SqlConnection { ConnectionString = connStr };
            conn.Open();
            return conn;
        }
    }
}
