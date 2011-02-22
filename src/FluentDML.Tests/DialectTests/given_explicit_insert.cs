using System;
using System.Data;
using NUnit.Framework;

namespace FluentDML.Tests.DialectTests
{

    public abstract class given_explicit_insert : DialectTestFixture
    {

        protected override IDbCommand GetCommand()
        {
            return DB().Insert<Customer>()
                .Set(c => c.CustomerId, _customer.CustomerId)
                .Set(c => c.Name, _customer.Name)
                .Set(c => c.Billing.City, _customer.Billing.City)
                .ToCommand();
        }

        private Customer _customer = new Customer()
                                         {
                                             Billing = new Address()
                                                           {
                                                               City = "Houston"
                                                           },
                                             CustomerId = Guid.NewGuid(),
                                             Name = "Jason"
                                         };

        [Test]
        public virtual void it_generates_insert_sql()
        {
            var cmd = GetCommand();
            var sql = cmd.CommandText;
            Assert.That(sql, Is.StringStarting("INSERT INTO [Customer] ("));
        }

        [Test]
        public virtual void it_generates_column_list()
        {
            var cmd = GetCommand();
            var sql = cmd.CommandText;
            Assert.That(sql, Is.StringContaining("[Billing_City],[CustomerId],[Name]"));
        }

        [Test]
        public virtual void it_generates_parameter_list()
        {
            var cmd = GetCommand();
            var sql = cmd.CommandText;
            Assert.That(sql, Is.StringContaining("@p0,@p1,@p2"));
        }

        [Test]
        public virtual void it_generates_where_parameters()
        {
            var cmd = GetCommand();
            Assert.That(GetParam(cmd, "p0").Value, Is.EqualTo(_customer.Billing.City));
            Assert.That(GetParam(cmd, "p1").Value, Is.EqualTo(_customer.CustomerId));
            Assert.That(GetParam(cmd, "p2").Value, Is.EqualTo(_customer.Name));
        }

        [Test]
        public virtual void it_inserted_a_row()
        {
            int rows;
            var insert = GetCommand();

            using (var conn = GetOpenConnection())
            {
                insert.Connection = conn;
                rows = insert.ExecuteNonQuery();
                conn.Close();
            }
            Assert.That(rows, Is.EqualTo(1));
        }


    }
}
