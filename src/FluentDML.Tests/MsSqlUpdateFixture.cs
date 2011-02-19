using System.Data;
using FluentDML.Dialect;
using FluentDML.Mapping;
using NUnit.Framework;

namespace FluentDML.Tests
{
    [TestFixture]
    public class MsSqlUpdateFixture : BaseFixture
    {

        private IDbCommand GetBasicCommand()
        {
            var mapMaker = new DefaultMapMaker();
            var map = mapMaker.MakeMap(typeof (Customer));
            var db = new MsSqlDialect(map);
            return db.Update<Customer>()
                .Set(c => c.Name, "Jason")
                .Set(c => c.Address.City, "Houston")
                .Set(c => c.Address.State, "TX")
                .Where(c => c.Name == "")
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
            Assert.That(sql, Is.StringStarting("UPDATE [Customer] t SET"));
        }

        [Test]
        public void it_generates_set_clause()
        {
            var cmd = GetBasicCommand();
            var sql = cmd.CommandText;
            Assert.That(sql, Is.StringContaining("t.[Name] = @p0"));
            Assert.That(sql, Is.StringContaining("t.[Address_City] = @p1"));
            Assert.That(sql, Is.StringContaining("t.[Address_State] = @p2"));
        }

        [Test]
        public void it_generates_where_clause()
        {
            var cmd = GetBasicCommand();
            var sql = cmd.CommandText;
            Assert.That(sql, Is.StringContaining("WHERE (t.[Name] = @p3)"));
        }

        [Test]
        public void it_generates_set_parameters()
        {
            var cmd = GetBasicCommand();
            Assert.That(GetParam(cmd, "p0").Value, Is.EqualTo("Jason"));
            Assert.That(GetParam(cmd, "p1").Value, Is.EqualTo("Houston"));
            Assert.That(GetParam(cmd, "p2").Value, Is.EqualTo("TX"));
        }

        [Test]
        public void it_generates_where_parameters()
        {
            var cmd = GetBasicCommand();
            Assert.That(GetParam(cmd, "p3").Value, Is.EqualTo(""));
        }


        private class Customer
        {
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
