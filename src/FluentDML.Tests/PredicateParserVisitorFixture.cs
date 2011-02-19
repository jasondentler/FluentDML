using System;
using System.Linq.Expressions;
using FluentDML.Expressions;
using FluentDML.Expressions.AST;
using NUnit.Framework;

namespace FluentDML.Tests
{
    [TestFixture]
    public class PredicateParserVisitorFixture : BaseFixture
    {

        [Test]
        public void Can_parse_constant()
        {
            var expr = MakeExpression<Customer, bool>(c => true);
            var myExpr = PredicateParserVisitor.Parse(expr);
            var myConstant = myExpr as Constant;
            Assert.That(myExpr, Is.InstanceOf<Constant>());
            Assert.That(myConstant.Value, Is.True);
        }

        [Test]
        public void Can_parse_property()
        {
            var expr = MakeExpression<Customer, string>(c => c.Name);
            var myExpr = PredicateParserVisitor.Parse(expr);
            var myProp = myExpr as Property;
            Assert.That(myExpr, Is.InstanceOf<Property>());
            Assert.That(myProp.PropertyPath, Is.EqualTo("Name"));
        }

        [Test]
        public void Can_parse_property_equals_constant()
        {
            var expr = MakeExpression<Customer, bool>(c => c.Name == "Jason");
            var myExpr = PredicateParserVisitor.Parse(expr);
            var myBinary = myExpr as Binary;
            Assert.That(myExpr, Is.InstanceOf<Binary>());
            Assert.That(myBinary.Operation, Is.EqualTo(ExpressionType.Equal));
            Assert.That(myBinary.Left, Is.InstanceOf<Property>());
            Assert.That(myBinary.Right, Is.InstanceOf<Constant>());
        }


        private static Expression MakeExpression<T, TProperty>(Expression<Func<T, TProperty>> expression)
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
