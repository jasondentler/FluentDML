namespace FluentDML.Dialect
{
    public interface IDialect
    {
        IInsert<T> Insert<T>();
        IDelete<T> Delete<T>();
        IUpsert<T> Upsert<T>();
        IUpdate<T> Update<T>();

    }

}
