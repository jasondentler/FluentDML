using System.Linq.Expressions;

namespace FluentDML.Expressions.AST
{
    public class Unary : MyExpression
    {

        public Unary(UnaryExpression expression)
        {
            Expression = PredicateParserVisitor.Parse(expression.Operand);
            Operation = expression.NodeType;
        }

        public MyExpression Expression { get; set; }
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