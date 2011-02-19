using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using FluentDML.Expressions;
using FluentDML.Expressions.AST;
using FluentDML.Mapping;

namespace FluentDML.Dialect
{
    public abstract class BaseSqlDelete<T> : BaseSql<T>,
        IDelete<T>,
        IDeleteWhere<T>
    {

        private readonly List<SimpleExpression> _predicates;

        protected BaseSqlDelete(ClassMap map)
            : base(map)
        {
            _predicates = new List<SimpleExpression>();
        }


        public IDeleteWhere<T> Where(Expression<Func<T, bool>> predicate)
        {
            _predicates.Add(PredicateParserVisitor.Parse(predicate));
            return this;
        }

        public IDeleteWhere<T> And(Expression<Func<T, bool>> predicate)
        {
            return Where(predicate);
        }

        public IDbCommand ToCommand()
        {
            return ToCommand(Map.TableName, And(_predicates));
        }

        protected abstract IDbCommand ToCommand(
            string tableName,
            SimpleExpression predicate);

    }
}
