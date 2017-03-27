using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace GraphQL.BatchResolver
{
    /// <summary>
    /// Adds support for resolving batches from an inner object.
    /// </summary>
    public class BatchFieldResolver<TSource, TKey, TReturn> : IFieldResolver
    {
        private readonly ConditionalWeakTable<object, ILookup<TKey, TReturn>> _resultsTable = new ConditionalWeakTable<object, ILookup<TKey, TReturn>>();
        private readonly Func<ResolveFieldContext<IEnumerable<TKey>>, ILookup<TKey, TReturn>> _resolver;
        private readonly Func<TSource, TKey> _keySelector;

        public BatchFieldResolver(Func<ResolveFieldContext<IEnumerable<TKey>>, ILookup<TKey, TReturn>> resolver, Func<TSource, TKey> keySelector)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            _resolver = resolver;
            _keySelector = keySelector;
        }

        public object Resolve(ResolveFieldContext context)
        {
            return ResolveInternal(context);
        }

        public IEnumerable<TReturn> ResolveInternal(ResolveFieldContext context)
        {
            var key = _keySelector((TSource)context.Source);

            var tableKey = context.Document;
            ILookup<TKey, TReturn> result = null;
            if (_resultsTable.TryGetValue(tableKey, out result))
                return result[key];

            var currentBatch = BatchStack.Peek();
            context.Source = BatchStack.Peek().OfType<TSource>().Select(_keySelector).Distinct();
            result = _resolver(new ResolveFieldContext<IEnumerable<TKey>>(context));
            _resultsTable.Add(tableKey, result);
            return BatchStack.Push(result.SelectMany(group => group), result[key]);
        }
    }
}