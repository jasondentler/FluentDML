using System;
using FluentDML.Mapping;

namespace FluentDML.Dialect
{
    public abstract class BaseDialect : IDialect
    {
        private readonly Map _map;

        protected BaseDialect(Map map)
        {
            _map = map;
        }

        public IDelete<T> Delete<T>()
        {
            throw new NotImplementedException();
        }

        public IUpsert<T> Upsert<T>()
        {
            throw new NotImplementedException();
        }

        public IUpdate<T> Update<T>()
        {
            return CreateSqlUpdate<T>(_map.GetClassMap<T>());
        }

        protected abstract IUpdate<T> CreateSqlUpdate<T>(ClassMap map);

    }
}
