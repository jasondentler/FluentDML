using System;

namespace FluentDML.Expressions.AST
{
    public class Constant : SimpleExpression
    {

        public object Value { get; set; }

        public Constant(object constant)
        {
            Value = constant ?? DBNull.Value;
        }
    }
}
