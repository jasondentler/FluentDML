namespace FluentDML.Dialect
{
    public interface IDialect
    {

        IDelete<T> Delete<T>();
        IUpsert<T> Upsert<T>();
        IUpdate<T> Update<T>();

    }

}
