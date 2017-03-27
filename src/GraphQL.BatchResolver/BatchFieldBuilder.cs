using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Builders;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace GraphQL.BatchResolver
{
    public static class FieldBuilderExtensions
    {
        public static FieldBuilder<TSource, object> Resolve<TSource, TReturn>(this FieldBuilder<TSource, object> builder, Func<ResolveFieldContext, IEnumerable<TReturn>> resolve)
        {
            return builder.Resolve(new FuncFieldResolver<IEnumerable<TReturn>>(ctx => BatchStack.Push(resolve(ctx))));
        }

        public static FieldBuilder<TSource, object> ResolveMany<TSource, TKey, TReturn>(this FieldBuilder<TSource, object> builder, Func<TSource, TKey> keySelector, Func<ResolveFieldContext<IEnumerable<TKey>>, ILookup<TKey, TReturn>> resolve)
        {
            return builder.Resolve(new ChildBatchResolver<TSource, TKey, TReturn>(resolve, keySelector));
        }
    }
}