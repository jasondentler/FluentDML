using System;
using System.Linq.Expressions;

namespace FluentDML
{
    public interface IUpsertSet<T>
    {
        IUpsertSet<T> Set<TProperty>(Expression<Func<T, TProperty>> property, TProperty value);
        IUpsertWhere<T> Where(Expression<Func<T, bool>> predicate);
    }
}