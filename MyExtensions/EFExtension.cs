using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Transactions;
using System.Reflection;
using System.Linq.Expressions;
using System.Data;
using System.Data.Objects.DataClasses;
using System.Data.Objects;

namespace MyExtensions
{
    public static class EFExtension
    {
        public static void AddRange<TEntity>(this ICollection<TEntity> collection, IEnumerable<TEntity> entities)
            where TEntity : class
        {
            foreach (TEntity e in entities)
            {
                collection.Add(e);
            }
        }

        public static void AddRange<TEntity>(this DbSet<TEntity> dbset, IEnumerable<TEntity> entities)
            where TEntity : class
        {
            foreach (TEntity e in entities)
            {
                dbset.Add(e);
            }
        }

        /// <summary>
        /// Esegue l'operazione per tutte le entità nella in entities, salvando ad ogni pagina
        /// </summary>
        /// <remarks>http://stackoverflow.com/questions/5940225/fastest-way-of-inserting-in-entity-framework/5942176#5942176</remarks>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="context"></param>
        /// <param name="entities"></param>
        /// <param name="action"></param>
        public static void BulkOperation<TEntity>(this DbContext context, IOrderedQueryable<TEntity> entities, Action<TEntity> action, int pagingCount = 100)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(15)))
            {
                var entitiesCount = entities.Count();
                var pagesCount = (int)Math.Ceiling((decimal)entitiesCount / (decimal)pagingCount);

                for (int i = pagesCount - 1; i >= 0; i--)
                {
                    var pagedEntities = entities.Page(i, pagingCount).ToList();
                    foreach (var entity in pagedEntities)
                    {
                        action(entity);
                    }
                    context.SaveChanges();
                    // clear the entities tracking
                    context.ChangeTracker.Entries().ToList().ForEach(e => e.State = EntityState.Detached);
                }

                scope.Complete();
            }
        }

        /// <summary>
        /// Permette di ottenere la query associata ad una proprietà di navigazione, "da dentro" l'entità
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="entity"></param>
        /// <param name="navigationProperty"></param>
        /// <returns></returns>
        public static IQueryable<TElement> GetIQueryable<TEntity, TElement>(this TEntity entity, Expression<Func<TEntity, ICollection<TElement>>> navigationProperty)
            where TEntity : class
            where TElement : class
        {
            var aa = entity.GetType().GetField("_entityWrapper").GetValue(entity);
            var bb = aa.GetType().GetProperty("Context").GetValue(aa) as System.Data.Objects.ObjectContext;
            using (var dbContext = new DbContext(bb, false))
            {
                var query = dbContext.Entry(entity).Collection(navigationProperty).Query();
                return query;
            }
        }

        public static TContext GetEntities<TContext>(this IEntityWithRelationships entity) where TContext : class
        {
            return GetContext(entity) as TContext;
        }

        public static object GetContext(this IEntityWithRelationships entity)
        {

            if (entity == null)
                throw new ArgumentNullException("entity");

            var relationshipManager = entity.RelationshipManager;

            var relatedEnd = relationshipManager.GetAllRelatedEnds()
                                                .FirstOrDefault();

            if (relatedEnd == null)
                throw new Exception("No relationships found");

            var query = relatedEnd.CreateSourceQuery() as ObjectQuery;

            if (query == null)
                throw new Exception("The Entity is Detached");

            return query.Context;
        }
    }
}
