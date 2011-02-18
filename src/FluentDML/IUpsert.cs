using System;
using System.Linq.Expressions;
using System.Data;

namespace FluentDML
{
    public interface IUpsert<T>
    {

        IUpsertWhere<T> MapFrom<TSource>(TSource source);
        IUpsertSet<T> Set<TProperty>(Expression<Func<T, TProperty>> property, TProperty value);

    }
}
