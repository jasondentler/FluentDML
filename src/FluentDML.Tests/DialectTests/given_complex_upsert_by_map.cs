using System;
using System.Data;
using AutoMapper;
using NUnit.Framework;

namespace FluentDML.Tests.DialectTests
{

    public abstract class given_complex_upsert_by_map : DialectTestFixture
    {
        protected given_complex_upsert_by_map(DialectTestFixtureConfiguration cfg) : base(cfg)
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

        private CustomerMovedEvent _event = new CustomerMovedEvent() { EventSourceId = Guid.NewGuid(), BillingCity = "Austin" };

        protected override IDbCommand GetCommand()
        {
            return DB().Upsert<Customer>()
                .MapFrom(_event)
                .WithId(c => c.CustomerId);
        }

        [Test]
        public virtual void it_inserted_a_row()
        {
            var clear = DB().Delete<Customer>()
                .Where(c => true)
                .ToCommand();


            var upsert = GetCommand();

            using (var conn = GetOpenConnection())
            {
                clear.Connection = conn;
                clear.ExecuteNonQuery();

                upsert.Connection = conn;
                var rows = upsert.ExecuteNonQuery();
                Assert.That(rows, Is.EqualTo(1));

                var countQuery = conn.CreateCommand();
                countQuery.CommandText = "select count(*) from Customer";
                var count = countQuery.ExecuteScalar();
                Assert.That(count.ToString(), Is.EqualTo("1"));

                var nameQuery = conn.CreateCommand();
                nameQuery.CommandText = "select Name from Customer";
                var name = nameQuery.ExecuteScalar();
                Assert.That(name, Is.EqualTo(DBNull.Value));

                var billingCityQuery = conn.CreateCommand();
                billingCityQuery.CommandText = "select Billing_City from Customer";
                var billingCity = billingCityQuery.ExecuteScalar();
                Assert.That(billingCity.ToString(), Is.EqualTo(_event.BillingCity));


                conn.Close();
            }
        }


        [Test]
        public virtual void it_updated_a_row()
        {
            var clear = DB().Delete<Customer>()
                .Where(c => true)
                .ToCommand();

            var insert = DB().Insert<Customer>()
                .Set(c => c.CustomerId, _event.EventSourceId)
                .Set(c => c.Name, "Jason")
                .Set(c => c.Billing.City, "Not Austin")
                .ToCommand();

            var update = GetCommand();
            
            using (var conn = GetOpenConnection())
            {
                clear.Connection = conn;
                clear.ExecuteNonQuery();

                insert.Connection = conn;
                var rows = insert.ExecuteNonQuery();
                if (rows != 1)
                    Assert.Inconclusive("Insert statement failed. Can't continue testing update.");

                update.Connection = conn;
                rows = update.ExecuteNonQuery();
                Assert.That(rows, Is.EqualTo(1));

                var countQuery = conn.CreateCommand();
                countQuery.CommandText = "select count(*) from Customer";
                var count = countQuery.ExecuteScalar();
                Assert.That(count.ToString(), Is.EqualTo("1"));

                var nameQuery = conn.CreateCommand();
                nameQuery.CommandText = "select Name from Customer";
                var name = nameQuery.ExecuteScalar();
                Assert.That(name.ToString(), Is.EqualTo("Jason"));

                var billingCityQuery = conn.CreateCommand();
                billingCityQuery.CommandText = "select Billing_City from Customer";
                var billingCity = billingCityQuery.ExecuteScalar();
                Assert.That(billingCity.ToString(), Is.EqualTo(_event.BillingCity));

                conn.Close();
            }
        }


    }
}
