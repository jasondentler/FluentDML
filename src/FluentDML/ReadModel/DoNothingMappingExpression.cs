using System;
using AutoMapper;

namespace FluentDML.ReadModel
{

    /// <summary>
    /// Fake mapping expression returned from CreateMap when a mapping already exists.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    public class DoNothingMappingExpression<TSource, TDestination> :
        IMappingExpression<TSource, TDestination>
    {

        public IMappingExpression<TSource, TDestination> AfterMap<TMappingAction>() where TMappingAction : IMappingAction<TSource, TDestination>
        {
            return this;
        }

        public IMappingExpression<TSource, TDestination> AfterMap(Action<TSource, TDestination> afterFunction)
        {
            return this;
        }

        public IMappingExpression<TSource, TDestination> BeforeMap<TMappingAction>() where TMappingAction : IMappingAction<TSource, TDestination>
        {
            return this;
        }

        public IMappingExpression<TSource, TDestination> BeforeMap(Action<TSource, TDestination> beforeFunction)
        {
            return this;
        }

        public IMappingExpression<TSource, TDestination> ConstructUsing(Func<TSource, TDestination> ctor)
        {
            return this;
        }

        public void ConvertUsing<TTypeConverter>() where TTypeConverter : ITypeConverter<TSource, TDestination>
        {
        }

        public void ConvertUsing(ITypeConverter<TSource, TDestination> converter)
        {
        }

        public void ConvertUsing(Func<TSource, TDestination> mappingFunction)
        {
        }

        public void ForAllMembers(Action<IMemberConfigurationExpression<TSource>> memberOptions)
        {
        }

        public IMappingExpression<TSource, TDestination> ForMember(string name, Action<IMemberConfigurationExpression<TSource>> memberOptions)
        {
            return this;
        }

        public IMappingExpression<TSource, TDestination> ForMember(System.Linq.Expressions.Expression<Func<TDestination, object>> destinationMember, Action<IMemberConfigurationExpression<TSource>> memberOptions)
        {
            return this;
        }

        public IMappingExpression<TSource, TDestination> Include<TOtherSource, TOtherDestination>()
            where TOtherSource : TSource
            where TOtherDestination : TDestination
        {
            return this;
        }

        public IMappingExpression<TSource, TDestination> WithProfile(string profileName)
        {
            return this;
        }

    }

}
