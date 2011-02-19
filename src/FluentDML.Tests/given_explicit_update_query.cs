using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using FluentDML.Dialect;
using FluentDML.Mapping;
using NUnit.Framework;

namespace FluentDML.Tests
{
    [TestFixture]
    public class given_explicit_update_query : BaseFixture
    {

        private IDbCommand GetBasicCommand()
        {
            var mapMaker = new DefaultMapMaker();
            var map = mapMaker.MakeMap(typeof (Customer));
            var db = new MsSqlDialect(map);
            var id = new Guid();
            return db.Update<Customer>()
                .Set(c => c.Name, "Jason")
                .Set(c => c.Billing.City, "Houston")
                .Where(c => c.CustomerId == id)
                .ToCommand();
        }

        private IDbDataParameter GetParam(IDbCommand command, string name)
        {
            return (IDbDataParameter) command.Parameters[name];
        }

        [Test]
        public void it_generates_update_sql()
        {
            var cmd = GetBasicCommand();
            var sql = cmd.CommandText;
            Assert.That(sql, Is.StringStarting("UPDATE [Customer] SET"));
        }

        [Test]
        public void it_generates_set_clause()
        {
            var cmd = GetBasicCommand();
            var sql = cmd.CommandText;
            Assert.That(sql, Is.StringContaining("[Name] = @p0"));
            Assert.That(sql, Is.StringContaining("[Billing_City] = @p1"));
        }

        [Test]
        public void it_generates_where_clause()
        {
            var cmd = GetBasicCommand();
            var sql = cmd.CommandText;
            Assert.That(sql, Is.StringContaining("WHERE ([CustomerId] = @p2)"));
        }

        [Test]
        public void it_generates_set_parameters()
        {
            var cmd = GetBasicCommand();
            Assert.That(GetParam(cmd, "p0").Value, Is.EqualTo("Jason"));
            Assert.That(GetParam(cmd, "p1").Value, Is.EqualTo("Houston"));
        }

        [Test]
        public void it_generates_where_parameters()
        {
            var cmd = GetBasicCommand();
            Assert.That(GetParam(cmd, "p2").Value, Is.EqualTo(new Guid()));
        }

        [Test]
        public void it_updated_a_row()
        {
            int rows;
            var id = Guid.NewGuid();
            var mapMaker = new DefaultMapMaker();
            var map = mapMaker.MakeMap(typeof (Customer));
            var db = new MsSqlDialect(map);
            var insert =
                new SqlCommand(
                    "insert into customer (customerid, name, billing_city) values (@p0, @p1, @p2)");
            insert.Parameters.AddWithValue("p0", id);
            insert.Parameters.AddWithValue("p1", "Not Jason");
            insert.Parameters.AddWithValue("p2", "Not Houston");

            var connStr = ConfigurationManager.ConnectionStrings["db"].ConnectionString;
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                insert.Connection = conn;
                insert.ExecuteNonQuery();

                var update = db.Update<Customer>()
                    .Set(c => c.Name, "Jason")
                    .Set(c => c.Billing.City, "Houston")
                    .Where(c => c.CustomerId == id)
                    .ToCommand();

                Debug.WriteLine(update.CommandText);

                update.Connection = conn;
                rows = update.ExecuteNonQuery();
                conn.Close();
            }

            Assert.That(rows, Is.EqualTo(1));

        }

        protected override void OnFixtureTearDown()
        {
            var mapMaker = new DefaultMapMaker();
            var map = mapMaker.MakeMap(typeof(Customer));
            var db = new MsSqlDialect(map);
            var connStr = ConfigurationManager.ConnectionStrings["db"].ConnectionString;
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                var cmd = db.Delete<Customer>().Where(c => true).ToCommand();
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            base.OnFixtureTearDown();
        }


    }
}
