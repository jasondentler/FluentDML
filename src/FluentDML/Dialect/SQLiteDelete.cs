using System;
using System.Data;
using System.Text;
using FluentDML.Expressions.AST;
using FluentDML.Mapping;

namespace FluentDML.Dialect
{

    public class SQLiteDelete<T> : BaseSqlDelete<T>
    {
        private readonly Func<IDbCommand> _commandConstructor;

        public SQLiteDelete(Func<IDbCommand> commandConstructor, ClassMap map)
            : base(map)
        {
            _commandConstructor = commandConstructor;
        }

        protected override IDbCommand ToCommand(string tableName, SimpleExpression predicate)
        {
            var cmd = _commandConstructor();
            cmd.CommandType = CommandType.Text;
            BuildSql(cmd, tableName, predicate);
            return cmd;
        }

        private void BuildSql(IDbCommand cmd, string tableName, SimpleExpression predicate)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("DELETE FROM [{0}] ", tableName);
            sb.Append(Convert(predicate, cmd));
            cmd.CommandText = sb.ToString();
        }
    }

}
