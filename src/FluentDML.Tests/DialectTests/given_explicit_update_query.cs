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
    public abstract class given_explicit_update_query : DialectTestFixture
    {

        protected Guid Id { get; private set; }

        public given_explicit_update_query()
        {
            Id = Guid.NewGuid();
        }

        protected override IDbCommand GetCommand()
        {
            return DB().Update<Customer>()
                .Set(c => c.Name, "Jason")
                .Set(c => c.Billing.City, "Houston")
                .Where(c => c.CustomerId == Id)
                .ToCommand();
        }

        [Test]
        public virtual void it_generates_update_sql()
        {
            var cmd = GetCommand();
            var sql = cmd.CommandText;
            Assert.That(sql, Is.StringStarting("UPDATE [Customer] SET"));
        }

        [Test]
        public virtual void it_generates_set_clause()
        {
            var cmd = GetCommand();
            var sql = cmd.CommandText;
            Assert.That(sql, Is.StringContaining("[Name] = @p0"));
            Assert.That(sql, Is.StringContaining("[Billing_City] = @p1"));
        }

        [Test]
        public virtual void it_generates_where_clause()
        {
            var cmd = GetCommand();
            var sql = cmd.CommandText;
            Assert.That(sql, Is.StringContaining("WHERE ([CustomerId] = @p2)"));
        }

        [Test]
        public virtual void it_generates_set_parameters()
        {
            var cmd = GetCommand();
            Assert.That(GetParam(cmd, "p0").Value, Is.EqualTo("Jason"));
            Assert.That(GetParam(cmd, "p1").Value, Is.EqualTo("Houston"));
        }

        [Test]
        public virtual void it_generates_where_parameters()
        {
            var cmd = GetCommand();
            Assert.That(GetParam(cmd, "p2").Value, Is.EqualTo(Id));
        }

        [Test]
        public virtual void it_updated_a_row()
        {
            var id = Guid.NewGuid();
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
