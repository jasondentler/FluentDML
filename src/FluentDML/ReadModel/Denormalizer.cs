using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using FluentDML.Dialect;

namespace FluentDML.ReadModel
{
    public abstract class Denormalizer<TEntity> : IDenormalizer
    {
        private readonly IDialect _dialect;
        private readonly string _connectionStringName;

        /// <summary>
        /// Constructor for Denormalizer base type
        /// </summary>
        /// <param name="dialect">FluentDML dialect singleton</param>
        /// <param name="connectionStringName">connection string name from app.config or web.config</param>
        protected Denormalizer(IDialect dialect, string connectionStringName)
        {
            _dialect = dialect;
            _connectionStringName = connectionStringName;
        }

        /// <summary>
        /// Returns the id property of the entity
        /// </summary>
        /// <returns>The id property of the entity</returns>
        protected abstract Expression<Func<TEntity, object>> GetId();

        /// <summary>
        /// Creates an automapper mapping from TEvent to TEntity
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns>An IMappingExpression allowing you to further define the mapping from TEvent to TEntity</returns>
        /// <remarks>If the exact mapping already exists, a DoNothingMappingExpression is returned.
        /// If found, the existing AutoMapper mapping is not affected.</remarks>
        protected IMappingExpression<TEvent, TEntity> CreateMap<TEvent>()
        {
            var maps = Mapper.GetAllTypeMaps();
            var map = maps.Where(m =>
                                 (Type) m.SourceType == typeof (TEvent) &&
                                 (Type) m.DestinationType == typeof (TEntity))
                .FirstOrDefault();
            return map == null
                       ? Mapper.CreateMap<TEvent, TEntity>()
                       : new DoNothingMappingExpression<TEvent, TEntity>();
        }

        /// <summary>
        /// Performs an Upsert on an entity, using data from the given event
        /// </summary>
        /// <typeparam name="TEvent">The type of event being handled</typeparam>
        /// <param name="event">The event being handled</param>
        protected void Upsert<TEvent>(TEvent @event)
        {
            _dialect.Upsert(@event, GetId(), _connectionStringName);
        }

        /// <summary>
        /// Performs an Insert on an entity, using data from the given event
        /// </summary>
        /// <typeparam name="TEvent">The type of event being handled</typeparam>
        /// <param name="event">The event being handled</param>
        protected void Insert<TEvent>(TEvent @event)
        {
            _dialect.Insert(@event, GetId(), _connectionStringName);
        }

        /// <summary>
        /// Performs an Update on an entity, using data from the given event
        /// </summary>
        /// <typeparam name="TEvent">The type of event being handled</typeparam>
        /// <param name="event">The event being handled</param>
        protected void Update<TEvent>(TEvent @event)
        {
            _dialect.Update(@event, GetId(), _connectionStringName);
        }

        protected IUpdate<TEntity> Update()
        {
            return _dialect.Update<TEntity>();
        }

        protected IInsert<TEntity> Insert()
        {
            return _dialect.Insert<TEntity>();
        }

        protected IUpsert<TEntity> Upsert()
        {
            return _dialect.Upsert<TEntity>();
        }

        protected IDelete<TEntity> Delete()
        {
            return _dialect.Delete<TEntity>();
        }

        protected int Execute(IDbCommand command)
        {
            return _dialect.Execute(_connectionStringName, command);
        }

    }
}
