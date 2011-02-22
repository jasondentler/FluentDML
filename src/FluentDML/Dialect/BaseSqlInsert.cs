using System.Collections.Generic;
using System.Data;
using System.Linq;
using FluentDML.AutoMapped;
using FluentDML.Mapping;

namespace FluentDML.Dialect
{
    public abstract class BaseSqlInsert<T> : BaseSql<T>, IInsert<T>
    {
        protected BaseSqlInsert(ClassMap map) : base(map)
        {
        }

        public virtual IDbCommand MapFrom<TSource>(TSource source)
        {
            var valueMap = DictionaryMapper.GetValueMap<TSource, T>(source, Map);
            var columnMap = valueMap.ToDictionary(
                kv => Map.GetColumnName(kv.Key),
                kv => kv.Value);

            return ToCommand(Map.TableName, columnMap);
        }

        protected abstract IDbCommand ToCommand(
            string tableName,
            Dictionary<string, object> columnMap);

    }
}
