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
    public class MsSqlDeleteFixture : BaseFixture
    {

        private IDbCommand GetBasicCommand()
        {
            var mapMaker = new DefaultMapMaker();
            var map = mapMaker.MakeMap(typeof (Customer));
            var db = new MsSqlDialect(map);
            var id = new Guid();
            return db.Delete<Customer>()
                .Where(c => c.CustomerId == id)
                .ToCommand();
        }

        private IDbDataParameter GetParam(IDbCommand command, string name)
        {
            return (IDbDataParameter) command.Parameters[name];
        }

        [Test]
        public void it_generates_delete_sql()
        {
            var cmd = GetBasicCommand();
            var sql = cmd.CommandText;
            Assert.That(sql, Is.StringStarting("DELETE FROM [Customer] WHERE"));
        }

        [Test]
        public void it_generates_where_clause()
        {
            var cmd = GetBasicCommand();
            var sql = cmd.CommandText;
            Assert.That(sql, Is.StringContaining("WHERE ([CustomerId] = @p0)"));
        }

        [Test]
        public void it_generates_where_parameters()
        {
            var cmd = GetBasicCommand();
            Assert.That(GetParam(cmd, "p0").Value, Is.EqualTo(new Guid()));
        }

        [Test]
        public void it_deletes_a_row()
        {
            int rows;
            var id = Guid.NewGuid();
            var mapMaker = new DefaultMapMaker();
            var map = mapMaker.MakeMap(typeof (Customer));
            var db = new MsSqlDialect(map);
            var insert =
                new SqlCommand(
                    "insert into customer (customerid, name, address_city, address_state) values (@p0, @p1, @p2, @p3)");
            insert.Parameters.AddWithValue("p0", id);
            insert.Parameters.AddWithValue("p1", "Not Jason");
            insert.Parameters.AddWithValue("p2", "Not Houston");
            insert.Parameters.AddWithValue("p3", "Not Texas");

            var connStr = ConfigurationManager.ConnectionStrings["db"].ConnectionString;
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                insert.Connection = conn;
                insert.ExecuteNonQuery();

                var delete = db.Delete<Customer>()
                    .Where(c => c.CustomerId == id)
                    .ToCommand();

                Debug.WriteLine(delete.CommandText);

                delete.Connection = conn;
                rows = delete.ExecuteNonQuery();
                conn.Close();
            }

            Assert.That(rows, Is.EqualTo(1));

        }


        private class Customer
        {
            public Guid CustomerId { get; set; }
            public string Name { get; set; }
            public Address Address { get; set; }
        }

        private class Address
        {
            public string City { get; set; }
            public string State { get; set; }
        }


    }
}
