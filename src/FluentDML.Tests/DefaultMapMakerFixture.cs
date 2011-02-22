using FluentDML.Mapping;
using NUnit.Framework;

namespace FluentDML.Tests
{
    [TestFixture]
    public class DefaultMapMakerFixture : BaseFixture
    {

        [Test]
        public void Can_map_basic_class()
        {
            var mapMaker = new DefaultMapMaker();
            var map = mapMaker.MakeMap(typeof (Address));
            var classMap = map.GetClassMap<Address>();
            Assert.That(classMap.MappedType, Is.EqualTo(typeof (Address)));
        }

        [Test]
        public void MappedType_is_mapped_type()
        {
            var mapMaker = new DefaultMapMaker();
            var map = mapMaker.MakeMap(typeof(Address));
            var classMap = map.GetClassMap<Address>();
            Assert.That(classMap.MappedType, Is.EqualTo(typeof(Address)));
        }

        [Test]
        public void TableName_is_simple_class_name()
        {
            var mapMaker = new DefaultMapMaker();
            var map = mapMaker.MakeMap(typeof(Address));
            var classMap = map.GetClassMap<Address>();
            Assert.That(classMap.TableName, Is.EqualTo("Address"));
        }

        [Test]
        public void City_property_by_string_is_mapped_to_City_column()
        {
            var mapMaker = new DefaultMapMaker();
            var map = mapMaker.MakeMap(typeof(Address));
            var classMap = map.GetClassMap<Address>();
            var cityColumn = classMap.GetColumnName("City");
            Assert.That(cityColumn, Is.EqualTo("City"));
        }

        [Test]
        public void City_property_by_expression_is_mapped_to_City_column()
        {
            var mapMaker = new DefaultMapMaker();
            var map = mapMaker.MakeMap(typeof(Address));
            var classMap = map.GetClassMap<Address>();
            var cityColumn = classMap.GetColumnName<Address, string>(a => a.City);
            Assert.That(cityColumn, Is.EqualTo("City"));
        }

        [Test]
        public void State_property_is_mapped_to_State_column()
        {
            var mapMaker = new DefaultMapMaker();
            var map = mapMaker.MakeMap(typeof(Address));
            var classMap = map.GetClassMap<Address>();
            var stateColumn = classMap.GetColumnName<Address, string>(a => a.State);
            Assert.That(stateColumn, Is.EqualTo("State"));
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
