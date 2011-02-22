using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using FluentDML.Mapping;

namespace FluentDML.Dialect
{
    public class SQLiteInsert<T> : BaseSqlInsert<T>
    {
        private readonly Func<IDbCommand> _commandConstructor;
        protected readonly Dictionary<string, string> ColumnToParameterMap;

        public SQLiteInsert(Func<IDbCommand> commandConstructor, ClassMap map) : base(map)
        {
            _commandConstructor = commandConstructor;
            ColumnToParameterMap = new Dictionary<string, string>();
        }
        
        protected override IDbCommand ToCommand(
            string tableName, 
            Dictionary<string, object> columnMap)
        {
            var sb = new StringBuilder();
            var cmd = _commandConstructor();
            cmd.CommandType = CommandType.Text;

            foreach (var column in columnMap.OrderBy(kv => kv.Key))
                SetParameter(cmd, column.Value, column.Key);

            sb.AppendFormat("INSERT INTO [{0}] (", Map.TableName);
            sb.Append(string.Join(",", ColumnToParameterMap
                                           .OrderBy(kv => kv.Key)
                                           .Select(kv => string.Format("[{0}]", kv.Key))));

            sb.Append(") VALUES (");
            sb.Append(string.Join(",", ColumnToParameterMap
                                           .OrderBy(kv => kv.Key)
                                           .Select(kv => string.Format("@{0}", kv.Value))));
            sb.Append(")");
            cmd.CommandText = sb.ToString();
            return cmd;
        }
        
        protected override string SetParameter(IDbCommand command, object value, string columnName)
        {
            var paramName = base.SetParameter(command, value, columnName);
            ColumnToParameterMap[columnName] = paramName;
            return paramName;
        }
        
    }
}
