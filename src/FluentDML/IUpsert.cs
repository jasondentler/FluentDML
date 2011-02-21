using System;
using System.Linq.Expressions;

namespace FluentDML
{

    public interface IUpsert<T>
    {
        IUpsertSet<T> Set<TProperty>(Expression<Func<T, TProperty>> property, TProperty value);
        IUpsertMap<T> MapFrom<TSource>(TSource source);
    }
}
