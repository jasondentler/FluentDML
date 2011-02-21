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
    public class MsSqlUpsert<T> : BaseSqlUpsert<T>
    {
        private readonly Func<IDbCommand> _commandConstructor;
        protected readonly Dictionary<string, string> ColumnToParameterMap;

        public MsSqlUpsert(Func<IDbCommand> commandConstructor, ClassMap map) : base(map)
        {
            _commandConstructor = commandConstructor;
            ColumnToParameterMap = new Dictionary<string, string>();
        }

        protected virtual void ValidatePredicate(SimpleExpression predicate)
        {
            if (!(predicate is Binary))
                throw new NotSupportedException("Upsert predicate must be simple Property == Constant");
            var equality = (Binary) predicate;
            if (equality.Operation != ExpressionType.Equal)
                throw new NotSupportedException("Upsert predicate must be simple Property == Constant");
            var left = equality.Left as Property;
            var right = equality.Right as Constant;
            if (left == null)
                throw new NotSupportedException("Upsert predicate must be simple Property == Constant");
            if (right == null)
                throw new NotSupportedException("Upsert predicate must be simple Property == Constant");
        }

        protected override IDbCommand ToCommand(
            string tableName, 
            Dictionary<string, object> columnMap, 
            SimpleExpression predicate)
        {
            ValidatePredicate(predicate);

            var equality = (Binary) predicate;
            var left = (Property) equality.Left;
            var predicateColumn = Map.GetColumnName(left.PropertyPath);

            var cmd = _commandConstructor();
            cmd.CommandType = CommandType.Text;

            var sb = new StringBuilder();

            sb.AppendLine("BEGIN TRAN;");
            sb.AppendFormat("UPDATE [{0}] WITH (SERIALIZABLE) SET \r\n", tableName);
            sb.Append(BuildSetList(columnMap, cmd, ColumnToParameterMap));
            sb.AppendLine();
            sb.Append(Convert(predicate, cmd));

            // Get the parameter created when converting the predicate
            ColumnToParameterMap[predicateColumn] = ColumnToParameterMap[string.Empty];
            ColumnToParameterMap.Remove(string.Empty);

            sb.AppendLine();
            sb.AppendLine("IF @@ROWCOUNT = 0");
            sb.AppendLine();
            sb.AppendFormat("    INSERT INTO [{0}] (", tableName);
            sb.AppendLine();
            sb.Append(string.Join(",\r\n", ColumnToParameterMap.Keys
                                               .OrderBy(col => col)
                                               .Select(col => string.Format("      [{0}]", col))));
            sb.AppendLine();
            sb.AppendLine("    ) VALUES (");
            sb.Append(string.Join(",\r\n", ColumnToParameterMap
                                               .OrderBy(item => item.Key)
                                               .Select(item => string.Format("       @{0}", item.Value))));
            sb.AppendLine(");");
            sb.AppendLine();
            sb.AppendLine("IF @@ROWCOUNT <> 1");
            sb.AppendLine("BEGIN");
            sb.AppendLine("   COMMIT TRAN;");
            sb.AppendLine("END");
            sb.AppendLine("ELSE");
            sb.AppendLine("   ROLLBACK TRAN");

            cmd.CommandText = sb.ToString();
            return cmd;
        }

        protected virtual string BuildSetList(
            Dictionary<string, object> columnMap,
            IDbCommand command,
            Dictionary<string, string> columnToParameterMap)
        {
            var setListParts = (from column in columnMap
                                let paramName = SetParameter(command, column.Value, column.Key)
                                select string.Format("[{0}]=@{1} ", column.Key, paramName)).ToList();
            return string.Join(",\r\n", setListParts);
        }

        protected override string SetParameter(IDbCommand command, object value, string columnName)
        {
            var paramName =  base.SetParameter(command, value, columnName);
            ColumnToParameterMap[columnName] = paramName;
            return paramName;
        }

    }
}
