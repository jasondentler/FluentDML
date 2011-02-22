using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using FluentDML.AutoMapped;
using FluentDML.Expressions;
using FluentDML.Mapping;

namespace FluentDML.Dialect
{
    public abstract class BaseSqlInsert<T> : BaseSql<T>, IInsert<T>, IInsertSet<T>
    {

        private readonly Dictionary<string, object> _setMap;

        protected BaseSqlInsert(ClassMap map) : base(map)
        {
            _setMap = new Dictionary<string, object>();
        }

        public virtual IDbCommand MapFrom<TSource>(TSource source)
        {
            var valueMap = DictionaryMapper.GetValueMap<TSource, T>(source, Map);
            foreach (var item in valueMap)
                _setMap.Add(item.Key, item.Value);
            return ToCommand();
        }

        public IInsertSet<T> Set<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            _setMap.Add(FindMemberVisitor.FindMember(property), value);
            return this;
        }

        public IDbCommand ToCommand()
        {
            var columnMap = _setMap.ToDictionary(item => Map.GetColumnName(item.Key), item => item.Value);
            return ToCommand(Map.TableName, columnMap);
        }

        protected abstract IDbCommand ToCommand(
            string tableName,
            Dictionary<string, object> columnMap);

    }
}
