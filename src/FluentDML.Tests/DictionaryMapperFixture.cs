using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using FluentDML.AutoMapped;
using FluentDML.Expressions;
using FluentDML.Mapping;
using NUnit.Framework;

namespace FluentDML.Tests
{
    [TestFixture]
    public class DictionaryMapperFixture : BaseFixture
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
                .ForMember(c => c.Billing, mo => mo.MapFrom(e => new Address() {City = e.BillingCity}));
            Mapper.AssertConfigurationIsValid();
        }

        [Test]
        public void it_can_map_simple_types()
        {
            var mapMaker = new DefaultMapMaker();
            var map = mapMaker.MakeMap(typeof(Customer));
            var evnt = new CustomerCreatedEvent() { EventSourceId = Guid.NewGuid(), Name = "Jason Dentler" };
            var values = DictionaryMapper.GetValueMap<CustomerCreatedEvent, Customer>(evnt, map);

            Assert.That(values.Count, Is.EqualTo(2));
            Assert.That(values["CustomerId"], Is.EqualTo(evnt.EventSourceId));
            Assert.That(values["Name"], Is.EqualTo(evnt.Name));
        }

        [Test]
        public void it_can_map_types_with_components()
        {
            var mapMaker = new DefaultMapMaker();
            var map = mapMaker.MakeMap(typeof(Customer));
            var evnt = new CustomerMovedEvent() {EventSourceId = Guid.NewGuid(), BillingCity = "Austin"};
            var values = DictionaryMapper.GetValueMap<CustomerMovedEvent, Customer>(evnt, map);

            Assert.That(values.Count, Is.EqualTo(2));
            Assert.That(values["CustomerId"], Is.EqualTo(evnt.EventSourceId));
            Assert.That(values["Billing.City"], Is.EqualTo(evnt.BillingCity));
        }

    }
}
