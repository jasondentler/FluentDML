using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentDML.NHibernateAdapter;
using NUnit.Framework;

namespace FluentDML.Tests.NHibernateAdapter
{
    [TestFixture]
    public class MapMakerFixture : BaseFixture
    {

        protected override void OnFixtureSetup()
        {
            var cfg = NHibernateConfiguration.Configuration;
            base.OnFixtureSetup();
        }

        [Test]
        public void it_maps_customer()
        {
            var mapMaker = new NHibernateMapMaker(NHibernateConfiguration.Configuration);
            var map = mapMaker.MakeMap();
            var classMap = map.GetClassMap<Customer>();
            Assert.Pass();
        }

        [Test]
        public void it_maps_customer_to_customer_type()
        {
            var mapMaker = new NHibernateMapMaker(NHibernateConfiguration.Configuration);
            var map = mapMaker.MakeMap();
            var classMap = map.GetClassMap<Customer>();
            Assert.That(classMap.MappedType, Is.EqualTo(typeof(Customer)));
        }

        [Test]
        public void it_maps_customer_to_customer_table()
        {
            var mapMaker = new NHibernateMapMaker(NHibernateConfiguration.Configuration);
            var map = mapMaker.MakeMap();
            var classMap = map.GetClassMap<Customer>();
            Assert.That(classMap.TableName, Is.EqualTo("Customer"));
        }

        [Test]
        public void it_doesnt_map_anything_else()
        {
            var mapMaker = new NHibernateMapMaker(NHibernateConfiguration.Configuration);
            var map = mapMaker.MakeMap();
            var classMap = map.GetClassMap<Customer>();
            Assert.That(classMap.GetMappedProperties().Count(), Is.EqualTo(4));
        }

        [Test]
        public void it_maps_CustomerId()
        {
            var mapMaker = new NHibernateMapMaker(NHibernateConfiguration.Configuration);
            var map = mapMaker.MakeMap();
            var classMap = map.GetClassMap<Customer>();
            Assert.That(classMap.GetColumnName<Customer, Guid>(c => c.CustomerId), Is.EqualTo("CustomerId"));
        }

        [Test]
        public void it_maps_Name()
        {
            var mapMaker = new NHibernateMapMaker(NHibernateConfiguration.Configuration);
            var map = mapMaker.MakeMap();
            var classMap = map.GetClassMap<Customer>();
            Assert.That(classMap.GetColumnName<Customer, string>(c => c.Name), Is.EqualTo("Name"));
        }

        [Test]
        public void it_maps_BillingCity()
        {
            var mapMaker = new NHibernateMapMaker(NHibernateConfiguration.Configuration);
            var map = mapMaker.MakeMap();
            var classMap = map.GetClassMap<Customer>();
            Assert.That(classMap.GetColumnName<Customer, string>(c => c.Billing.City), Is.EqualTo("Billing_City"));
        }



    }
}
