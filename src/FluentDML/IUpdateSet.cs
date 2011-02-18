using System;
using System.Linq.Expressions;

namespace FluentDML
{
    public interface IUpdateSet<T>
    {

        IUpdateSet<T> Set<TProperty>(Expression<Func<T, TProperty>> property, TProperty value);
        IUpdateWhere<T> Where(Expression<Func<T, bool>> predicate);

    }
}
