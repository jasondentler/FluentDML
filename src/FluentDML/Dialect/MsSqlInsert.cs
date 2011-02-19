using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using FluentDML.AutoMapped;
using FluentDML.Mapping;

namespace FluentDML.Dialect
{
    public class MsSqlInsert<T> : BaseSqlInsert<T>
    {
        private readonly Func<IDbCommand> _commandConstructor;

        public MsSqlInsert(Func<IDbCommand> commandConstructor, ClassMap map) : base(map)
        {
            _commandConstructor = commandConstructor;
        }

        public override IDbCommand MapFrom<TSource>(TSource source)
        {
            var valueMap = DictionaryMapper.GetValueMap<TSource, T>(source, Map);
            var sb = new StringBuilder();
            var cmd = _commandConstructor();
            cmd.CommandType = CommandType.Text;

            sb.AppendFormat("INSERT INTO [{0}] (", Map.TableName);
            sb.Append(string.Join(",", valueMap.Select(kv => ColumnMap(kv, cmd)).ToArray()));
            sb.Append(") VALUES (");
            sb.Append(string.Join(",", IntSequence(0, cmd.Parameters.Count).Select(i => "@p" + i).ToArray()));
            sb.Append(")");
            cmd.CommandText = sb.ToString();
            return cmd;
        }

        private string ColumnMap(KeyValuePair<string, object> item, IDbCommand command)
        {
            var columnName = Map.GetColumnName(item.Key);
            var parmName = SetParameter(command, item.Value);
            return string.Format("[{0}]", columnName);
        }

        private IEnumerable<int> IntSequence(int fromInclusive, int toExclusive)
        {
            for (var i = fromInclusive; i < toExclusive; i++)
                yield return i;
        }

    }
}
