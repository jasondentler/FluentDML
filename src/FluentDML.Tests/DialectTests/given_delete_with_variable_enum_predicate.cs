using System;
using System.Data;
using NUnit.Framework;

namespace FluentDML.Tests.DialectTests
{
    public abstract class given_delete_with_variable_enum_predicate : DialectTestFixture
    {
        protected given_delete_with_variable_enum_predicate(DialectTestFixtureConfiguration cfg) : base(cfg)
        {
            Id = Guid.NewGuid();
            Day = DayOfWeek.Thursday;
        }

        protected Guid Id { get; private set; }
        protected DayOfWeek Day { get; private set; }

        protected override IDbCommand GetCommand()
        {
            return DB().Delete<Customer>()
                .Where(c => c.DayOfTheWeek == Day)
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
            Assert.That(sql, Is.StringContaining("WHERE ([DayOfTheWeek] = @p0)"));
        }

        [Test]
        public virtual void it_generates_where_parameters()
        {
            var cmd = GetCommand();
            Assert.That(GetParam(cmd, "p0").Value, Is.EqualTo(Day));
        }

        [Test]
        public virtual void it_deleted_a_row()
        {
            IDbCommand insert = DB().Insert<Customer>()
                .Set(c => c.CustomerId, Id)
                .Set(c => c.Name, "Jason")
                .Set(c => c.Billing.City, "Not Houston")
                .Set(c => c.DayOfTheWeek, Day)
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
