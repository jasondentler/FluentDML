using System;
using System.Data;
using NUnit.Framework;

namespace FluentDML.Tests.DialectTests
{
    public abstract class given_delete_with_complex_where : DialectTestFixture
    {
        protected given_delete_with_complex_where(DialectTestFixtureConfiguration cfg) : base(cfg)
        {
            Id = Guid.NewGuid();
            Name = "Jason";
        }

        protected Guid Id { get; private set; }
        protected string Name { get; private set; }

        protected override IDbCommand GetCommand()
        {
            return DB().Delete<Customer>()
                .Where(c => c.CustomerId == Id && c.Name == Name)
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
            Assert.That(sql, Is.StringContaining("WHERE (([CustomerId] = @p0) AND ([Name] = @p1))"));
        }

        [Test]
        public virtual void it_generates_where_parameters()
        {
            var cmd = GetCommand();
            Assert.That(GetParam(cmd, "p0").Value, Is.EqualTo(Id));
            Assert.That(GetParam(cmd, "p1").Value, Is.EqualTo(Name));
        }

        [Test]
        public virtual void it_deleted_a_row()
        {
            IDbCommand insert = DB().Insert<Customer>()
                .Set(c => c.CustomerId, Id)
                .Set(c => c.Name, "Jason")
                .Set(c => c.Billing.City, "Not Houston")
                .ToCommand();

            var delete = GetCommand();

            using (var conn = GetOpenConnection())
            {
                insert.Connection = conn;
                var rows = insert.ExecuteNonQuery();
                if (rows != 1)
                    Assert.Inconclusive("Insert statement failed. Can't continue testing update.");

                delete.Connection = conn;
                rows = delete.ExecuteNonQuery();
                Assert.That(rows, Is.EqualTo(1));
                conn.Close();
            }
        }

    }
}
