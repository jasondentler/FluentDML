namespace FluentDML.Dialect
{
    public interface IDialect
    {
        IInsert<T> Insert<T>();
        IDelete<T> Delete<T>();
        IUpdate<T> Update<T>();
        IUpsert<T> Upsert<T>();

    }

}
