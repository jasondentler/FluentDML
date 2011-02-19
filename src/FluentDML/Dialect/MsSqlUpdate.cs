using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using FluentDML.Expressions.AST;
using FluentDML.Mapping;

namespace FluentDML.Dialect
{
    public class MsSqlUpdate<T> : BaseSqlUpdate<T>
    {
        private readonly Func<IDbCommand> _commandConstructor;

        public MsSqlUpdate(Func<IDbCommand> commandConstructor, ClassMap map) : base(map)
        {
            _commandConstructor = commandConstructor;
        }

        protected override IDbCommand ToCommand(
            string tableName,
            Dictionary<string, object> set, 
            SimpleExpression predicate)
        {
            var cmd = _commandConstructor();
            cmd.CommandType = CommandType.Text;
            BuildSql(cmd, tableName, set, predicate);
            return cmd;
        }

        protected virtual void BuildSql(
            IDbCommand command, 
            string tableName,
            Dictionary<string, object> set,
            SimpleExpression predicate)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("UPDATE [{0}] t SET", tableName);

            var paramIndex = 0;
            foreach (var item in set)
            {
                sb.AppendLine(paramIndex == 0 ? "" : ",");
                sb.AppendFormat("t.[{0}] = @p{1}", item.Key, command.Parameters.Count);
                SetParameter(command, item.Value);
            }
            sb.AppendLine();
            sb.Append(Convert(predicate, command));
            command.CommandText = sb.ToString();
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
            var sql = new StringBuilder();
            sql.Append("WHERE ");
            Convert(predicate, command, sql);
            return sql.ToString();
        }

        protected virtual void Convert(SimpleExpression predicate,  IDbCommand command, StringBuilder sql)
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
            sql.Append("t.[");
            sql.Append(Map.GetColumnName(property.PropertyPath));
            sql.Append("]");
        }

        protected virtual void Convert(Constant constant, IDbCommand command, StringBuilder sql)
        {
            var paramName = SetParameter(command, constant.Value);
            sql.Append("@");
            sql.Append(paramName);
        }


    }
}