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

        public Guid Id { get; private set; }

        public PredicateParserVisitorFixture()
        {
            Id = Guid.NewGuid();
        }

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

        [Test]
        public void Can_parse_variable_to_constant()
        {
            var id = new Guid();
            var expr = MakeExpression<Customer, Guid>(c => id);
            var myExpr = PredicateParserVisitor.Parse(expr);
            var myConstant = myExpr as Constant;
            Assert.That(myExpr, Is.InstanceOf<Constant>());
            Assert.That(myConstant.Value, Is.EqualTo(new Guid()));
        }

        [Test]
        public void Can_parse_unrelated_property_to_constant()
        {
            var expr = MakeExpression<Customer, Guid>(c => Id);
            var myExpr = PredicateParserVisitor.Parse(expr);
            var myConstant = myExpr as Constant;
            Assert.That(myExpr, Is.InstanceOf<Constant>());
            Assert.That(myConstant.Value, Is.EqualTo(Id));
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
