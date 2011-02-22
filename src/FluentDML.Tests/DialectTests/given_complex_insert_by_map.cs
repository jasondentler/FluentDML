using System;
using System.Data;
using AutoMapper;
using NUnit.Framework;

namespace FluentDML.Tests.DialectTests
{

    public abstract class given_complex_insert_by_map : DialectTestFixture
    {
        protected given_complex_insert_by_map(DialectTestFixtureConfiguration cfg) : base(cfg)
        {
        }

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

        protected override IDbCommand GetCommand()
        {
            return DB().Insert<Customer>()
                .MapFrom(_event);
        }

        protected CustomerMovedEvent _event = new CustomerMovedEvent()
                                                {EventSourceId = Guid.NewGuid(), BillingCity = "Austin"};

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
            Assert.That(sql, Is.StringContaining("[Billing_City],[CustomerId]"));
        }

        [Test]
        public virtual void it_generates_parameter_list()
        {
            var cmd = GetCommand();
            var sql = cmd.CommandText;
            Assert.That(sql, Is.StringContaining("@p0,@p1"));
        }

        [Test]
        public virtual void it_generates_where_parameters()
        {
            var cmd = GetCommand();
            Assert.That(GetParam(cmd, "p0").Value, Is.EqualTo(_event.BillingCity));
            Assert.That(GetParam(cmd, "p1").Value, Is.EqualTo(_event.EventSourceId));
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
