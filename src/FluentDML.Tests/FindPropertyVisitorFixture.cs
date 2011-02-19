using System;
using System.Linq.Expressions;
using FluentDML.Mapping;
using NUnit.Framework;

namespace FluentDML.Tests
{
    [TestFixture]
    public class FindMemberVisitorFixture : BaseFixture
    {

        [Test]
        public void Can_get_simple_property()
        {
            var expression = MakeExpression<Customer>(c => c.Name);
            var result = FindMemberVisitor.FindMember(expression);
            Assert.That(result, Is.EqualTo("Name"));
        }

        [Test]
        public void Can_get_secondary_property()
        {
            var expression = MakeExpression<Customer>(c => c.Address.City);
            var result = FindMemberVisitor.FindMember(expression);
            Assert.That(result, Is.EqualTo("Address.City"));
        }

        private static Expression MakeExpression<T>(Expression<Func<T, object>> expression)
        {
            return expression;
        }

        private class Customer
        {
            public string Name { get; set; }
            public Address Address { get; set; }
        }

        private class Address
        {
            public string City { get; set; }
        }

    }
}
