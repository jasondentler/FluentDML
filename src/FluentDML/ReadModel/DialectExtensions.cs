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
            return Execute(dialect, connectionStringName, cmd);
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
            return Execute(dialect, connectionStringName, cmd);
        }

        public static int Insert<TEntity, TEvent>(
            this IDialect dialect,
            TEvent @event,
            Expression<Func<TEntity, object>> id,
            string connectionStringName)
        {
            var cmd = dialect.Insert<TEntity>()
                .MapFrom(@event);
            return Execute(dialect, connectionStringName, cmd);
        }

        private static int Execute(IDialect dialect, string connectionStringName,  IDbCommand command)
        {
            int rowsAffected;
            using (var conn = dialect.GetConnection(connectionStringName))
            {
                conn.Open();
                rowsAffected = Execute(conn, command);
                conn.Close();
            }
            return rowsAffected;
        }

        private static int Execute(IDbConnection connection, IDbCommand command)
        {
            command.Connection = connection;
            return Execute(command);
        }

        private static int Execute(IDbCommand command)
        {
            return command.ExecuteNonQuery();
        }


    }
}
