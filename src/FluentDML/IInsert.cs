using System.Data;

namespace FluentDML
{
    public interface IInsert<T>
    {

        IDbCommand MapFrom<TSource>(TSource source);

    }
}
