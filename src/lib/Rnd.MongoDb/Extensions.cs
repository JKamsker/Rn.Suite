using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Rnd.MongoDb
{
    public static class Extensions
    {
        public static IMongoQueryable<TSource> MongoWhere<TSource>
        (
            this IMongoQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate
        ) => source.Where(predicate);

        public static IMongoQueryable<TSource> MongoTake<TSource>
        (
            this IMongoQueryable<TSource> source,
            int count
        )
        {
            return MongoQueryable.Take(source, count);
        }
        public static FilterDefinition<T> Where<T>(this FilterDefinition<T> input, Expression<Func<T, bool>> expression)
        {
            return Builders<T>.Filter.And(input, Builders<T>.Filter.Where(expression));
        }

        public static async IAsyncEnumerable<T> EnumerateAsync<T>(this Task<IAsyncCursor<T>> cursorTask, [EnumeratorCancellation] CancellationToken token = default)
        {
            var cursor = await cursorTask;
            await foreach (var item in cursor.EnumerateAsync(token))
            {
                yield return item;
            }
        }

        public static async IAsyncEnumerable<T> EnumerateAsync<T>(this IAsyncCursor<T> cursor, [EnumeratorCancellation] CancellationToken token = default)
        {
            while (await cursor.MoveNextAsync())
            {
                foreach (var item in cursor.Current)
                {
                    yield return item;
                }
                token.ThrowIfCancellationRequested();
            }
        }
    }
}