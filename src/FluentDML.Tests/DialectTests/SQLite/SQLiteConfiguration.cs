using System.Configuration;
using System.Data;
using System.Data.SQLite;
using FluentDML.Dialect;
using FluentDML.Mapping;

namespace FluentDML.Tests.DialectTests.SQLite
{
    public class SQLiteConfiguration : DialectTestFixtureConfiguration
    {
        public override IDialect GetDialect(Map map)
        {
            return new SQLiteDialect(map);
        }

        public override IDbConnection GetOpenConnection()
        {
            var connStr = ConfigurationManager.ConnectionStrings["SQLite"].ConnectionString;
            var conn = new SQLiteConnection { ConnectionString = connStr };
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"CREATE TABLE Customer (CustomerId TEXT PRIMARY KEY, Name TEXT, Billing_City TEXT, DayOfTheWeek INTEGER)";
            cmd.ExecuteNonQuery();

            return conn;
        }
    }
}
