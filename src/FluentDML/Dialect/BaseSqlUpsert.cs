using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using FluentDML.AutoMapped;
using FluentDML.Expressions;
using FluentDML.Expressions.AST;
using FluentDML.Mapping;

namespace FluentDML.Dialect
{
    public abstract class BaseSqlUpsert<T> : 
        BaseSql<T>, 
        IUpsert<T>,
        IUpsertMap<T>,
        IUpsertSet<T>,
        IUpsertWhere<T>
    {

        protected readonly Dictionary<string, object> SetMap;
        protected readonly List<SimpleExpression> Predicates;

        protected BaseSqlUpsert(ClassMap map) : base(map)
        {
            SetMap = new Dictionary<string, object>();
            Predicates = new List<SimpleExpression>();
        }

        public virtual IUpsertSet<T> Set<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            var propertyPath = FindMemberVisitor.FindMember(property);
            SetMap[propertyPath] = value;
            return this;
        }

        public virtual IUpsertWhere<T> Where(Expression<Func<T, bool>> predicate)
        {
            Predicates.Add(PredicateParserVisitor.Parse(predicate));
            return this;
        }

        public virtual IUpsertMap<T> MapFrom<TSource>(TSource source)
        {
            var valueMap = DictionaryMapper.GetValueMap<TSource, T>(source, Map);
            foreach (var item in valueMap)
                SetMap[item.Key] = item.Value;
            return this;
        }

        public virtual IDbCommand WithId<TProperty>(Expression<Func<T, TProperty>> property)
        {
            var propertyPath = FindMemberVisitor.FindMember(property);
            var value = SetMap[propertyPath];
            SetMap.Remove(propertyPath);
            var predicate = new Binary(new Property(property), new Constant(value), ExpressionType.Equal);
            Predicates.Add(predicate);
            return ToCommand();
        }

        public virtual IUpsertWhere<T> And(Expression<Func<T, bool>> predicate)
        {
            Predicates.Add(PredicateParserVisitor.Parse(predicate));
            return this;
        }

        public virtual IDbCommand ToCommand()
        {
            var tableName = Map.TableName;
            var columnMap = SetMap.ToDictionary(item => Map.GetColumnName(item.Key), item => item.Value);
            var predicate = And(Predicates);
            return ToCommand(tableName, columnMap, predicate);
        }

        protected abstract IDbCommand ToCommand(string tableName,
                                                Dictionary<string, object> columnMap,
                                                SimpleExpression predicate);



    }
}
