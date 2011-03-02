using System;
using System.Data;
using FluentDML.Dialect;
using System.Linq.Expressions;

namespace FluentDML.ReadModel
{
    public static class DialectExtensions
    {

        public static int Upsert<TEntity, TEvent>(
            this IDialect dialect,
            TEvent @event, 
            Expression<Func<TEntity, object>> id,
            string connectionStringName)
        {
            var cmd = dialect.Upsert<TEntity>()
                .MapFrom(@event)
                .WithId(id);
            return dialect.Execute(connectionStringName, cmd);
        }

        public static int Update<TEntity, TEvent>(
            this IDialect dialect,
            TEvent @event,
            Expression<Func<TEntity, object>> id,
            string connectionStringName)
        {
            var cmd = dialect.Update<TEntity>()
                .MapFrom(@event)
                .WithId(id);
            return dialect.Execute(connectionStringName, cmd);
        }

        public static int Insert<TEntity, TEvent>(
            this IDialect dialect,
            TEvent @event,
            Expression<Func<TEntity, object>> id,
            string connectionStringName)
        {
            var cmd = dialect.Insert<TEntity>()
                .MapFrom(@event);
            return dialect.Execute(connectionStringName, cmd);
        }

        public static int Execute(this IDialect dialect, string connectionStringName,  IDbCommand command)
        {
            int rowsAffected;
            using (var conn = dialect.GetConnection(connectionStringName))
            {
                conn.Open();
                rowsAffected = conn.Execute(command);
                conn.Close();
            }
            return rowsAffected;
        }

        private static int Execute(this IDbConnection connection, IDbCommand command)
        {
            command.Connection = connection;
            return command.Execute();
        }

        private static int Execute(this IDbCommand command)
        {
            return command.ExecuteNonQuery();
        }


    }
}
