using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;

namespace MyExtensions
{
    public static class LinqExtension
    {
        public static TSource SearchBest<TSource>(this IEnumerable<TSource> source, Func<TSource, string> selector, string searchText, bool doContainsCheck = true)
        {
            searchText = searchText.ToLower().GetTextPart();
            Func<TSource, string> finalSelector = m => selector(m).GetTextPart().ToLower();

            if (doContainsCheck)
                source = source.Where(s => finalSelector(s).Contains(searchText) || searchText.Contains(finalSelector(s)));

            var option = source.Select(x => new { score = LevenshteinDistance.Compute(finalSelector(x), searchText), x });
            return option.WithMin(x => x.score).x;
        }

        public static IEnumerable<TSource> DuplicatedBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return source.GroupBy(keySelector).SelectMany(grp => grp.Skip(1));
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return source.GroupBy(keySelector).Select(grp => grp.First());
        }

        public static bool ContainsBy<TSource, TKey>(this IEnumerable<TSource> source, TSource value, Func<TSource, TKey> keySelector)
        {
            return source.GroupBy(keySelector).Select(grp => grp.Key).Contains(keySelector(value));
        }

        public static bool ContainsByRef<TSource, TKey>(this IEnumerable<TSource> source, TSource value, Func<TSource, TKey> keySelector)
        {
            return source.GroupBy(keySelector).Select(grp => grp.First()).Contains(value);
        }


        public static T WithMax<T, TValue>(this IEnumerable<T> source, Func<T, TValue> keySelector)
        {
            return source.OrderByDescending(keySelector).FirstOrDefault();
        }

        public static T WithMin<T, TValue>(this IEnumerable<T> source, Func<T, TValue> keySelector)
        {
            return source.OrderBy(keySelector).FirstOrDefault();
        }

        public static IEnumerable<TResult> LeftOuterJoin<TOuter, TLeft, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TLeft> left, Func<TOuter, TKey> outerKeySelector, Func<TLeft, TKey> leftKeySelector, Func<TOuter, TLeft, TResult> resultSelector)
        {
            return
              from o in outer
              join r in left on outerKeySelector(o) equals leftKeySelector(r) into j
              from r in j.DefaultIfEmpty()
              select resultSelector(o, r);
        }

        /// <summary>
        /// Da testare
        /// </summary>
        public static IEnumerable<TResult> LeftJoin<TOuter, TLeft, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TLeft> left, Func<TOuter, TKey> outerKeySelector, Func<TLeft, TKey> leftKeySelector, Func<TOuter, TLeft, TResult> resultSelector)
        {
            return LeftOuterJoin(outer, left, outerKeySelector, leftKeySelector, (o, r) => new { o = o, r = r }).Where(x => x.r == null).Select(x => resultSelector(x.o, x.r));
        }

        public static IEnumerable<TResult> Merge<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            var result = from f in first
                         join s in second on true equals true
                         select resultSelector(f, s);
            return result;
        }

        public static IEnumerable<TResult> Merge<TFirst, TSecond, TThird, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, IEnumerable<TThird> third, Func<TFirst, TSecond, TThird, TResult> resultSelector, Func<TFirst, TSecond, TThird, bool> resultFilter)
        {
            var result = from f in first
                         join s in second on true equals true
                         join t in third on true equals true
                         where resultFilter(f, s, t)
                         select resultSelector(f, s, t);
            return result;
        }

        public static TResult RandomOneOrDefault<TResult>(this IEnumerable<TResult> source)
        {
            return source.Random(1).FirstOrDefault();
        }
        
        public static IEnumerable<TResult> Random<TResult>(this IEnumerable<TResult> source, int count)
        {
            return source.Random().Take(count);
        }

        public static IEnumerable<TResult> Random<TResult>(this IEnumerable<TResult> source)
        {
            return source.OrderBy(s => Guid.NewGuid());
        }

        public static IQueryable<TResult> Page<TResult>(this IQueryable<TResult> source, int page, int pageSize)
        {
            return source.Skip((page) * pageSize).Take(pageSize);
        }

        public static IQueryable<T> WhereNot<T>(this IQueryable<T> queryable, Expression<Func<T, bool>> predicate)
        {
            return queryable.Where(predicate.Invert());
        }

        public static Expression<Func<T, bool>> Invert<T>(this Expression<Func<T, bool>> original)
        {
            return Expression.Lambda<Func<T, bool>>(Expression.Not(original.Body), original.Parameters.ToArray());
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action, Action variation, int executeVariationEvery)
        {
            int i = 0;
            foreach (var item in source)
            {
                action(item);
                i++;
                if ((i % executeVariationEvery) == 0)
                    variation();
            }
        }
    }
}