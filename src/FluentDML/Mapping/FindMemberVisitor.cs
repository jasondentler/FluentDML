using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FluentDML.Mapping
{
    public class FindMemberVisitor : ExpressionVisitor
    {

        private readonly Stack<MemberInfo> _result;

        private FindMemberVisitor()
        {
            _result = new Stack<MemberInfo>();
        }

        public static string FindMember(Expression expression)
        {
            var finder = new FindMemberVisitor();
            finder.Visit(expression);
            return string.Join(".", finder._result.Select(mi => mi.Name).ToArray());
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            _result.Push(m.Member);
            return base.VisitMemberAccess(m);
        }

    }
}
