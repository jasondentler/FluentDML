using System;
using System.Data.Common;

namespace FluentDML.Dialect
{
    public class AnsiSqlDialect : IDialect
    {
        private readonly DbProviderFactory _factory;

        public AnsiSqlDialect(DbProviderFactory factory)
        {
            _factory = factory;
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
            return new AnsiSqlUpdate<T>(_factory);
        }
    }
}
