using System.Linq.Expressions;
using FluentDML.Expressions.AST;

namespace FluentDML.Expressions
{
    public class PredicateParserVisitor : ExpressionVisitor
    {

        private MyExpression _expression;

        protected PredicateParserVisitor()
        {
        }

        public static MyExpression Parse(Expression expression)
        {
            var parser = new PredicateParserVisitor();
            parser.Visit(expression);
            return parser._expression;
        }


        protected override Expression VisitBinary(BinaryExpression b)
        {
            _expression = new Binary(b);
            return b;
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            _expression = new Unary(u);
            return u;
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            _expression = new Property(m);
            return m;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            _expression = new Constant(c.Value);
            return c;
        }


    }
}
