using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentDML.Expressions.AST;

namespace FluentDML.Expressions
{
    public class PredicateParserVisitor : ExpressionVisitor
    {

        private SimpleExpression _expression;

        protected PredicateParserVisitor()
        {
        }

        public static SimpleExpression Parse(Expression expression)
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
            if (m.Expression.NodeType == ExpressionType.Constant &&
                m.Member.MemberType == MemberTypes.Field)
            {
                VisitField(m);
                return m;
            }

            if (m.Expression.NodeType == ExpressionType.Constant &&
                m.Member.MemberType == MemberTypes.Property)
            {
                VisitUnrelatedProperty(m);
                return m;
            }

            _expression = new Property(m);
            return m;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            _expression = new Constant(c.Value);
            return c;
        }

        protected virtual Expression VisitField(MemberExpression m)
        {
            var constant = (ConstantExpression)m.Expression;
            var field = (FieldInfo)m.Member;
            _expression = new Constant(field.GetValue(constant.Value));
            return m;
        }

        protected virtual Expression VisitUnrelatedProperty(MemberExpression m)
        {
            var constant = (ConstantExpression)m.Expression;
            var member = (PropertyInfo) m.Member;
            var instance = constant.Value;
            var value = member.GetValue(instance, new object[0]);
            _expression = new Constant(value);
            return m;
        }


    }
}
