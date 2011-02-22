using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using FluentDML.Dialect;
using FluentDML.Mapping;
using NUnit.Framework;

namespace FluentDML.Tests.DialectTests.MsSql
{

    [TestFixture]
    public class given_explicit_insert : DialectTests.given_explicit_insert
    {

        protected override IDialect DB(Map map)
        {
            return new MsSqlDialect(map);
        }

        protected override IDbConnection GetOpenConnection()
        {
            var connStr = ConfigurationManager.ConnectionStrings["MsSql"].ConnectionString;
            var conn = new SqlConnection { ConnectionString = connStr };
            conn.Open();
            return conn;
        }

    }
}
