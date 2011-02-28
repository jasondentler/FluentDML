using System;
using System.Data;
using System.Data.Common;
using FluentDML.Mapping;

namespace FluentDML.Dialect
{
    public class SQLiteDialect : BaseDialect
    {
        private readonly DbProviderFactory _provider;

        public SQLiteDialect(Map map) : base(map)
        {
            _provider = DbProviderFactories.GetFactory("System.Data.SQLite");
        }

        protected IDbCommand CreateCommand()
        {
            return _provider.CreateCommand();
        }

        protected override IDbConnection CreateConnection()
        {
            return _provider.CreateConnection();
        }

        protected override IUpdate<T> CreateSqlUpdate<T>(ClassMap map)
        {
            return new SQLiteUpdate<T>(CreateCommand, map);
        }

        protected override IDelete<T> CreateSqlDelete<T>(ClassMap map)
        {
            return new SQLiteDelete<T>(CreateCommand, map);
        }

        protected override IInsert<T> CreateSqlInsert<T>(ClassMap map)
        {
            return new SQLiteInsert<T>(CreateCommand, map);
        }

        protected override IUpsert<T> CreateSqlUpsert<T>(ClassMap map)
        {
            return new SQLiteUpsert<T>(CreateCommand, map);
        }
    }
}
