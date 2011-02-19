using System;

namespace FluentDML.Expressions.AST
{
    public static class SimpleExpressionExtensions
    {

        public static bool IsDBNull(this SimpleExpression expr)
        {
            return expr as Constant == null
                       ? false
                       : ((Constant) expr).Value == DBNull.Value;

        }

    }
}
