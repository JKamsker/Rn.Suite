using System;
using System.Linq;
using System.Linq.Expressions;
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
    }
}