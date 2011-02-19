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
            MyExpression predicate)
        {
            var cmd = _commandConstructor();
            cmd.CommandType = CommandType.Text;
            BuildSql(cmd, tableName, set);
            return cmd;
        }

        protected virtual void BuildSql(
            IDbCommand command, 
            string tableName,
            Dictionary<string, object> set)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("UPDATE [{0}] t SET", tableName);

            var paramIndex = 0;
            foreach (var item in set)
            {
                sb.AppendLine(paramIndex == 0 ? "" : ",");
                sb.AppendFormat("t.[{0}] = @p{1}", item.Key, paramIndex);
                SetParameter(command, paramIndex++, item.Value);
            }

            command.CommandText = sb.ToString();
        }

        protected virtual void SetParameter(IDbCommand command, int paramIndex, object value)
        {
            var param = command.CreateParameter();
            param.ParameterName = "p" + paramIndex;
            param.Value = value;
            command.Parameters.Add(param);
        }



    }
}