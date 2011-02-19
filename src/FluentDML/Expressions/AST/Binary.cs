using System.Linq.Expressions;

namespace FluentDML.Expressions.AST
{

    public class Binary : SimpleExpression
    {

        public Binary(SimpleExpression left, SimpleExpression right, ExpressionType operation)
        {
            Left = left;
            Right = right;
            Operation = operation;
        }

        public Binary(BinaryExpression binaryExpression)
        {
            Left = PredicateParserVisitor.Parse(binaryExpression.Left);
            Right = PredicateParserVisitor.Parse(binaryExpression.Right);
            Operation = binaryExpression.NodeType;
        }

        public SimpleExpression Left { get; set; }
        public SimpleExpression Right { get; set; }
        public ExpressionType Operation { get; set; }

        //public enum BinaryOperations
        //{
        //    Add,
        //    AddChecked,
        //    Subtract,
        //    SubtractChecked,
        //    Multiply,
        //    MultiplyChecked,
        //    Divide,
        //    Modulo,
        //    And,
        //    AndAlso,
        //    Or,
        //    OrElse,
        //    LessThan,
        //    LessThanOrEqual,
        //    GreaterThan,
        //    GreaterThanOrEqual,
        //    Equal,
        //    NotEqual,
        //    Coalesce,
        //    ArrayIndex,
        //    RightShift,
        //    LeftShift,
        //    ExclusiveOr
        //}

    }
}
