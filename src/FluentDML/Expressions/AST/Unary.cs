using System.Linq.Expressions;

namespace FluentDML.Expressions.AST
{
    public class Unary : SimpleExpression
    {

        public Unary(UnaryExpression expression)
        {
            Expression = PredicateParserVisitor.Parse(expression.Operand);
            Operation = expression.NodeType;
        }

        public SimpleExpression Expression { get; set; }
        public ExpressionType Operation { get; set; }

        //public enum UnaryOperations
        //{
        //    Negate,
        //    NegateChecked,
        //    Not,
        //    Convert,
        //    ConvertChecked,
        //    ArrayLength,
        //    Quote,
        //    TypeAs
        //}
    }
}