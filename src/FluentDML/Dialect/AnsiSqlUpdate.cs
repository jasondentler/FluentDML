using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;

namespace FluentDML.Dialect
{
    public class AnsiSqlUpdate<T> 
        : IUpdate<T>
    {

        private Dictionary<string, object> _setMap;

        private readonly DbProviderFactory _factory;

        public AnsiSqlUpdate(DbProviderFactory factory)
        {
            _factory = factory;
            _setMap = new Dictionary<string, object>();
        }

        public IUpdateSet<T> Set<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            throw new NotImplementedException();
        }

        public IUpdateMap<T> MapFrom<TSource>(TSource source)
        {
            throw new NotImplementedException();
        }

    }
}
