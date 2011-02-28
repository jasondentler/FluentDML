using System.Configuration;
using System.Data;
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

        public virtual IDbConnection GetConnection(string connectionStringName)
        {
            var connStr = ConfigurationManager.ConnectionStrings[connectionStringName]
                .ConnectionString;
            var conn = CreateConnection();
            conn.ConnectionString = connStr;
            return conn;
        }

        protected abstract IDbConnection CreateConnection();

        public IInsert<T> Insert<T>()
        {
            return CreateSqlInsert<T>(_map.GetClassMap<T>());
        }

        public IDelete<T> Delete<T>()
        {
            return CreateSqlDelete<T>(_map.GetClassMap<T>());
        }

        public IUpdate<T> Update<T>()
        {
            return CreateSqlUpdate<T>(_map.GetClassMap<T>());
        }

        public IUpsert<T> Upsert<T>()
        {
            return CreateSqlUpsert<T>(_map.GetClassMap<T>());
        }

        protected abstract IUpdate<T> CreateSqlUpdate<T>(ClassMap map);
        protected abstract IDelete<T> CreateSqlDelete<T>(ClassMap map);
        protected abstract IInsert<T> CreateSqlInsert<T>(ClassMap map);
        protected abstract IUpsert<T> CreateSqlUpsert<T>(ClassMap map);

    }
}
