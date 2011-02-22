using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using FluentDML.Dialect;
using FluentDML.Mapping;
using NUnit.Framework;

namespace FluentDML.Tests.DialectTests
{
    public abstract class given_delete_by_id : DialectTestFixture
    {

        public given_delete_by_id()
        {
            Id = Guid.NewGuid();
        }

        protected Guid Id { get; private set; }

        protected override IDbCommand GetCommand()
        {
            return DB().Delete<Customer>()
                .Where(c => c.CustomerId == Id)
                .ToCommand();
        }


        [Test]
        public virtual void it_generates_delete_sql()
        {
            var cmd = GetCommand();
            var sql = cmd.CommandText;
            Assert.That(sql, Is.StringStarting("DELETE FROM [Customer] WHERE"));
        }

        [Test]
        public virtual void it_generates_where_clause()
        {
            var cmd = GetCommand();
            var sql = cmd.CommandText;
            Assert.That(sql, Is.StringContaining("WHERE ([CustomerId] = @p0)"));
        }

        [Test]
        public virtual void it_generates_where_parameters()
        {
            var cmd = GetCommand();
            Assert.That(GetParam(cmd, "p0").Value, Is.EqualTo(Id));
        }

        [Test]
        public virtual void it_updated_a_row()
        {
            IDbCommand insert = DB().Insert<Customer>()
                .Set(c => c.CustomerId, Id)
                .Set(c => c.Name, "Not Jason")
                .Set(c => c.Billing.City, "Not Houston")
                .ToCommand();

            var update = GetCommand();

            using (var conn = GetOpenConnection())
            {
                insert.Connection = conn;
                var rows = insert.ExecuteNonQuery();
                if (rows != 1)
                    Assert.Inconclusive("Insert statement failed. Can't continue testing update.");

                update.Connection = conn;
                rows = update.ExecuteNonQuery();
                Assert.That(rows, Is.EqualTo(1));
                conn.Close();
            }
        }

    }
}
