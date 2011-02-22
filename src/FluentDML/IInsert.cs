using System;
using System.Data;
using System.Linq.Expressions;

namespace FluentDML
{
    public interface IInsert<T>
    {

        IDbCommand MapFrom<TSource>(TSource source);
        IInsertSet<T> Set<TProperty>(Expression<Func<T, TProperty>> property, TProperty value);

    }
}
