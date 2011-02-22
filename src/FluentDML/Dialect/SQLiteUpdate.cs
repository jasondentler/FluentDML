using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using FluentDML.Expressions.AST;
using FluentDML.Mapping;

namespace FluentDML.Dialect
{
    public class SQLiteUpdate<T> : BaseSqlUpdate<T>
    {
        private readonly Func<IDbCommand> _commandConstructor;

        public SQLiteUpdate(Func<IDbCommand> commandConstructor, ClassMap map) : base(map)
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
            sb.AppendFormat("UPDATE [{0}] SET", tableName);

            foreach (var item in set)
            {
                sb.AppendLine(command.Parameters.Count == 0 ? "" : ",");
                sb.AppendFormat("[{0}] = @p{1}", item.Key, command.Parameters.Count);
                SetParameter(command, item.Value);
            }
            sb.AppendLine();
            sb.Append(Convert(predicate, command));
            command.CommandText = sb.ToString();
        }



    }
}