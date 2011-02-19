using System;
using System.Linq.Expressions;

namespace FluentDML
{
    public interface IUpdate<T>
    {

        IUpdateSet<T> Set<TProperty>(Expression<Func<T, TProperty>> property, TProperty value);
        //IUpdateMap<T> MapFrom<TSource>(TSource source);

    }
}
