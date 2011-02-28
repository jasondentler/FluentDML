using System.Data;

namespace FluentDML.Dialect
{
    public interface IDialect
    {

        IDbConnection GetConnection(string connectionStringName);
        IInsert<T> Insert<T>();
        IDelete<T> Delete<T>();
        IUpdate<T> Update<T>();
        IUpsert<T> Upsert<T>();

    }

}
