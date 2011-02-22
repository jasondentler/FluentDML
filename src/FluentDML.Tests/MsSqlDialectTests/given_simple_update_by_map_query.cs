using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using AutoMapper;
using FluentDML.Dialect;
using FluentDML.Mapping;
using NUnit.Framework;

namespace FluentDML.Tests.MsSqlDialectTests
{
    [TestFixture]
    public class given_simple_update_by_map_query : BaseFixture
    {

        protected override void OnFixtureSetup()
        {
            base.OnFixtureSetup();
            Mapper.Reset();
            Mapper.CreateMap<Address, Address>();
            Mapper.CreateMap<CustomerCreatedEvent, Customer>()
                .ForMember(c => c.CustomerId, mo => mo.MapFrom(e => e.EventSourceId))
                .ForMember(c => c.Billing, mo => mo.Ignore());
            Mapper.CreateMap<CustomerMovedEvent, Customer>()
                .ForMember(c => c.CustomerId, mo => mo.MapFrom(e => e.EventSourceId))
                .ForMember(c => c.Name, mo => mo.Ignore())
                .ForMember(c => c.Billing, mo => mo.MapFrom(e => new Address() { City = e.BillingCity }));
            Mapper.AssertConfigurationIsValid();
        }


        private IDbCommand GetCommand()
        {
            return GetCommand(_event);
        }

        private IDbCommand GetCommand<TEvent>(TEvent @event)
        {
            var mapMaker = new DefaultMapMaker();
            var map = mapMaker.MakeMap(typeof (Customer));
            var db = new MsSqlDialect(map);
            return db.Update<Customer>()
                .MapFrom(@event)
                .WithId(c => c.CustomerId);
        }

        private IDbDataParameter GetParam(IDbCommand command, string name)
        {
            return (IDbDataParameter) command.Parameters[name];
        }

        private CustomerCreatedEvent _event = new CustomerCreatedEvent()
                                                  {EventSourceId = Guid.NewGuid(), Name = "Jason"};

        [Test]
        public void it_generates_update_sql()
        {
            var cmd = GetCommand();
            var sql = cmd.CommandText;
            Assert.That(sql, Is.StringStarting("UPDATE [Customer] SET"));
        }

        [Test]
        public void it_generates_set_clause()
        {
            var cmd = GetCommand();
            var sql = cmd.CommandText;
            Assert.That(sql, Is.StringContaining("[Name] = @p0"));
        }

        [Test]
        public void it_generates_where_clause()
        {
            var cmd = GetCommand();
            var sql = cmd.CommandText;
            Assert.That(sql, Is.StringContaining("WHERE ([CustomerId] = @p1)"));
        }

        [Test]
        public void it_generates_set_parameters()
        {
            var cmd = GetCommand();
            Assert.That(GetParam(cmd, "p0").Value, Is.EqualTo(_event.Name));
        }

        [Test]
        public void it_generates_where_parameters()
        {
            var cmd = GetCommand();
            Assert.That(GetParam(cmd, "p1").Value, Is.EqualTo(_event.EventSourceId));
        }

        [Test]
        public void it_updated_a_row()
        {
            int rows;
            var id = Guid.NewGuid();
            var mapMaker = new DefaultMapMaker();
            var map = mapMaker.MakeMap(typeof(Customer));
            var db = new MsSqlDialect(map);
            var insert =
                new SqlCommand(
                    "insert into customer (customerid, name, billing_city) values (@p0, @p1, @p2)");
            insert.Parameters.AddWithValue("p0", _event.EventSourceId);
            insert.Parameters.AddWithValue("p1", "Not Jason");
            insert.Parameters.AddWithValue("p2", "Not Houston");

            var connStr = ConfigurationManager.ConnectionStrings["db"].ConnectionString;
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                insert.Connection = conn;
                insert.ExecuteNonQuery();

                var update = GetCommand();

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
