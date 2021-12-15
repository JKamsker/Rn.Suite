using AutoMapper;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rnd.Mapping.Automapper
{
    internal static class ProfileExtensions
    {
        /// <summary>
        /// same as
        /// .ForMember(x => x.Consumed, m => m.MapFrom(r=> r.NrOfTimesConsumed >= r.NrOfTimesConsumable));
        /// Usage:
        /// .SimpleMap(x => x.Consumed, x => x.NrOfTimesConsumed >= x.NrOfTimesConsumable);
        ///
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <typeparam name="TMember"></typeparam>
        /// <typeparam name="TSourceMember"></typeparam>
        /// <param name="expression"></param>
        /// <param name="destinationMember"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public static IMappingExpression<TSource, TDestination> SimpleMap<TSource, TDestination, TMember, TSourceMember>
        (
            this IMappingExpression<TSource, TDestination> expression,
            Expression<Func<TDestination, TMember>> destinationMember,
            Expression<Func<TSource, TSourceMember>> map

        )
        {
            return expression.ForMember(destinationMember, x => x.MapFrom(map));
        }

        /// <summary>
        /// same as
        /// .ForMember(x => x.Consumed, m => m.MapFrom(r=> r.NrOfTimesConsumed >= r.NrOfTimesConsumable));
        /// Usage:
        /// .SimpleMap(x => x.Consumed, x => x.NrOfTimesConsumed >= x.NrOfTimesConsumable);
        ///
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <typeparam name="TMember"></typeparam>
        /// <typeparam name="TSourceMember"></typeparam>
        /// <param name="expression"></param>
        /// <param name="destinationMember"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public static IMappingExpression<TSource, TDestination> SimpleMap<TSource, TDestination, TMember, TSourceMember>
        (
            this IMappingExpression<TSource, TDestination> expression,
            Expression<Func<TDestination, TMember>> destinationMember,
            Expression<Func<TSource, TSourceMember>> map,
            Func<TSource, TDestination, TMember, bool> condition
        ) => expression.ForMember(destinationMember, x =>
        {
            x.Condition(condition); //(a, b, c) => a.Reward is VoucherReward
            x.MapFrom(map);
        });

        public static IMappingExpression<TSource, TDestination> SimpleMap<TSource, TDestination, TMember, TSourceMember>
        (
            this IMappingExpression<TSource, TDestination> expression,
            Expression<Func<TDestination, TMember>> destinationMember,
            Expression<Func<TSource, TSourceMember>> map,
            Func<TSource, bool> condition
        ) => expression.ForMember(destinationMember, x =>
        {
            x.Condition(condition); //(a, b, c) => a.Reward is VoucherReward
            x.MapFrom(map);
        });

        public static IMappingExpression<TSource, TDestination> Ignore<TSource, TDestination, TMember>
        (
            this IMappingExpression<TSource, TDestination> expression,
            Expression<Func<TDestination, TMember>> destinationMember
        )
        {
            return expression.ForMember(destinationMember, x => x.Ignore());
        }
    }
}