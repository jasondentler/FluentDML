namespace FluentDML.Expressions.AST
{
    public class Constant : MyExpression
    {

        public object Value { get; set; }

        public Constant(object constant)
        {
            Value = constant;
        }
    }
}
