using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using AutoMapper;
using FluentDML.Dialect;
using FluentDML.Mapping;
using NUnit.Framework;

namespace FluentDML.Tests
{
    [TestFixture]
    public class given_simple_insert_by_map : BaseFixture
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
            return db.Insert<Customer>()
                .MapFrom(@event);
        }

        private IDbDataParameter GetParam(IDbCommand command, string name)
        {
            return (IDbDataParameter) command.Parameters[name];
        }

        private CustomerCreatedEvent _event = new CustomerCreatedEvent() { EventSourceId = Guid.NewGuid(), Name = "Jason" };


        [Test]
        public void it_generates_insert_sql()
        {
            var cmd = GetCommand();
            var sql = cmd.CommandText;
            Assert.That(sql, Is.StringStarting("INSERT INTO [Customer] ("));
        }

        [Test]
        public void it_generates_column_list()
        {
            var cmd = GetCommand();
            var sql = cmd.CommandText;
            Assert.That(sql, Is.StringContaining("[CustomerId],[Name]"));
        }

        [Test]
        public void it_generates_parameter_list()
        {
            var cmd = GetCommand();
            var sql = cmd.CommandText;
            Assert.That(sql, Is.StringContaining("@p0,@p1"));
        }

        [Test]
        public void it_generates_where_parameters()
        {
            var cmd = GetCommand();
            Assert.That(GetParam(cmd, "p0").Value, Is.EqualTo(_event.EventSourceId));
            Assert.That(GetParam(cmd, "p1").Value, Is.EqualTo(_event.Name));
        }

        [Test]
        public void it_insertd_a_row()
        {
            int rows;
            var id = Guid.NewGuid();
            var mapMaker = new DefaultMapMaker();
            var map = mapMaker.MakeMap(typeof(Customer));
            var db = new MsSqlDialect(map);
            var insert = db.Insert<Customer>()
                .MapFrom(_event);
            Debug.WriteLine(insert.CommandText);


            var connStr = ConfigurationManager.ConnectionStrings["db"].ConnectionString;
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                insert.Connection = conn;
                rows = insert.ExecuteNonQuery();
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
