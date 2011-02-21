using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using FluentDML.Mapping;

namespace FluentDML.Dialect
{
    public class MsSqlDialect : BaseDialect
    {
        private readonly Func<IDbCommand> _commandConstructor;

        public MsSqlDialect(Func<IDbCommand> commandConstructor, Map map)
            : base(map)
        {
            _commandConstructor = commandConstructor;
        }

        public MsSqlDialect(DbProviderFactory factory, Map map)
            : this(factory.CreateCommand, map)
        {
        }

        public MsSqlDialect(Map map)
            : this(SqlClientFactory.Instance, map)
        {
        }

        protected override IUpdate<T> CreateSqlUpdate<T>(ClassMap map)
        {
            return new MsSqlUpdate<T>(_commandConstructor, map);
        }

        protected override IDelete<T> CreateSqlDelete<T>(ClassMap map)
        {
            return new MsSqlDelete<T>(_commandConstructor, map);
        }

        protected override IInsert<T> CreateSqlInsert<T>(ClassMap map)
        {
            return new MsSqlInsert<T>(_commandConstructor, map);
        }

        protected override IUpsert<T> CreateSqlUpsert<T>(ClassMap map)
        {
            return new MsSqlUpsert<T>(_commandConstructor, map);
        }
    }
}
