using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FluentDML.Expressions.AST;
using FluentDML.Mapping;

namespace FluentDML.Dialect
{
    public abstract class BaseSql<T>
    {

        protected readonly ClassMap Map;

        protected BaseSql(ClassMap map)
        {
            Map = map;
        }

        protected virtual SimpleExpression And(IEnumerable<SimpleExpression> predicatesEnum)
        {
            if (!predicatesEnum.Any())
                return null;
            var q = new Queue<SimpleExpression>(predicatesEnum);
            var root = q.Dequeue();
            while (q.Any())
                root = new Binary(root, q.Dequeue(), ExpressionType.AndAlso);
            return root;
        }



        protected virtual string SetParameter(IDbCommand command, object value)
        {
            var param = command.CreateParameter();
            param.ParameterName = "p" + command.Parameters.Count;
            param.Value = value;
            command.Parameters.Add(param);
            return param.ParameterName;
        }

        protected virtual string Convert(SimpleExpression predicate, IDbCommand command)
        {
            if (predicate is Constant && (predicate as Constant).Value is bool)
            {
                var boolPredicate = (bool)((Constant)predicate).Value;
                return boolPredicate ? string.Empty : "WHERE 1=0";
            }
            var sql = new StringBuilder();
            sql.Append("WHERE ");
            Convert(predicate, command, sql);
            return sql.ToString();
        }

        protected virtual void Convert(SimpleExpression predicate, IDbCommand command, StringBuilder sql)
        {
            if (predicate as Binary != null)
                Convert((Binary)predicate, command, sql);
            if (predicate as Unary != null)
                Convert((Unary)predicate, command, sql);
            if (predicate as Property != null)
                Convert((Property)predicate, command, sql);
            if (predicate as Constant != null)
                Convert((Constant)predicate, command, sql);
        }

        protected virtual void Convert(Binary binary, IDbCommand command, StringBuilder sql)
        {
            switch (binary.Operation)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    Convert(" + ", binary.Left, binary.Right, command, sql);
                    return;
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    Convert(" - ", binary.Left, binary.Right, command, sql);
                    return;
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    Convert(" * ", binary.Left, binary.Right, command, sql);
                    return;
                case ExpressionType.Divide:
                    Convert(" / ", binary.Left, binary.Right, command, sql);
                    return;
                case ExpressionType.Modulo:
                    Convert(" % ", binary.Left, binary.Right, command, sql);
                    return;
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    Convert(" AND ", binary.Left, binary.Right, command, sql);
                    return;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    Convert(" OR ", binary.Left, binary.Right, command, sql);
                    return;
                case ExpressionType.LessThan:
                    Convert(" < ", binary.Left, binary.Right, command, sql);
                    return;
                case ExpressionType.LessThanOrEqual:
                    Convert(" <= ", binary.Left, binary.Right, command, sql);
                    return;
                case ExpressionType.GreaterThan:
                    Convert(" > ", binary.Left, binary.Right, command, sql);
                    return;
                case ExpressionType.GreaterThanOrEqual:
                    Convert(" >= ", binary.Left, binary.Right, command, sql);
                    return;
                case ExpressionType.Equal:
                    Equality(true, binary.Left, binary.Right, command, sql);
                    return;
                case ExpressionType.NotEqual:
                    Equality(false, binary.Left, binary.Right, command, sql);
                    return;
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    throw new NotSupportedException();
                default:
                    throw new NotSupportedException();
            }
        }

        protected virtual void Equality(bool equality, SimpleExpression left, SimpleExpression right, IDbCommand command, StringBuilder sql)
        {
            var op = equality ? " = " : " <> ";
            if (left.IsDBNull() && right.IsDBNull())
            {
                sql.Append(equality ? "(NULL IS NULL)" : "(NULL IS NOT NULL)");
            }
            else if (left.IsDBNull())
            {
                // Switch them so NULL is on the right.
                sql.Append("(");
                Convert(right, command, sql);
                sql.Append(equality ? " IS NULL)" : " IS NOT NULL)");
            }
            else if (right.IsDBNull())
            {
                sql.Append("(");
                Convert(left, command, sql);
                sql.Append(equality ? " IS NULL)" : " IS NOT NULL)");
            }
            else
            {
                Convert(op, left, right, command, sql);
            }
        }

        protected virtual void Convert(string operation, SimpleExpression left, SimpleExpression right, IDbCommand command, StringBuilder sql)
        {
            sql.Append("(");
            Convert(left, command, sql);
            sql.Append(operation);
            Convert(right, command, sql);
            sql.Append(")");
        }

        protected virtual void Convert(Unary unary, IDbCommand command, StringBuilder sql)
        {
            switch (unary.Operation)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                    sql.Append("-1 * ");
                    Convert(unary.Expression, command, sql);
                    return;
                case ExpressionType.Not:
                    sql.Append("NOT ");
                    Convert(unary.Expression, command, sql);
                    return;
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    throw new NotSupportedException();
                default:
                    throw new NotSupportedException();
            }
        }

        protected virtual void Convert(Property property, IDbCommand command, StringBuilder sql)
        {
            sql.Append("[");
            sql.Append(Map.GetColumnName(property.PropertyPath));
            sql.Append("]");
        }

        protected virtual void Convert(Constant constant, IDbCommand command, StringBuilder sql)
        {
            if (constant.Value is bool && (bool)constant.Value)
            {
                sql.Append("1");
                return;
            }

            if (constant.Value is bool && !(bool)constant.Value)
            {
                sql.Append("0");
                return;
            }

            var paramName = SetParameter(command, constant.Value);
            sql.Append("@");
            sql.Append(paramName);
        }


    }
}