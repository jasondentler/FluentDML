using System.Linq.Expressions;

namespace FluentDML.Expressions.AST
{
    public class Property : MyExpression
    {

        public Property(Expression expression)
        {
            PropertyPath = FindMemberVisitor.FindMember(expression);
        }

        public string PropertyPath { get; set; }

    }
}
