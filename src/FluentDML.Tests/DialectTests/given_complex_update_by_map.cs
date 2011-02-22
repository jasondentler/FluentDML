using System;
using System.Data;
using AutoMapper;
using NUnit.Framework;

namespace FluentDML.Tests.DialectTests
{

    public abstract class given_complex_update_by_map : DialectTestFixture
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
        
        private CustomerMovedEvent _event = new CustomerMovedEvent()
                                                {EventSourceId = Guid.NewGuid(), BillingCity = "Austin"};


        protected override IDbCommand GetCommand()
        {
            return DB().Update<Customer>()
                .MapFrom(_event)
                .WithId(c => c.CustomerId);
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
            Assert.That(sql, Is.StringContaining("[Billing_City] = @p0"));
        }

        [Test]
        public virtual void it_generates_where_clause()
        {
            var cmd = GetCommand();
            var sql = cmd.CommandText;
            Assert.That(sql, Is.StringContaining("WHERE ([CustomerId] = @p1)"));
        }

        [Test]
        public virtual void it_generates_set_parameters()
        {
            var cmd = GetCommand();
            Assert.That(GetParam(cmd, "p0").Value, Is.EqualTo(_event.BillingCity));
        }

        [Test]
        public virtual void it_generates_where_parameters()
        {
            var cmd = GetCommand();
            Assert.That(GetParam(cmd, "p1").Value, Is.EqualTo(_event.EventSourceId));
        }

        [Test]
        public virtual void it_updated_a_row()
        {
            var id = Guid.NewGuid();
            IDbCommand insert = DB().Insert<Customer>()
                .Set(c => c.CustomerId, _event.EventSourceId)
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
