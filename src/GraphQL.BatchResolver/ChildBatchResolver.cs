using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace GraphQL.BatchResolver
{
    /// <summary>
    /// Adds support for resolving batches from an inner object.
    /// </summary>
    public class ChildBatchResolver<TSource, TKey, TReturn> : IFieldResolver
    {
        private readonly ConditionalWeakTable<object, Task<ILookup<TKey, TReturn>>> _resultsTable = new ConditionalWeakTable<object, Task<ILookup<TKey, TReturn>>>();
        private readonly Func<ResolveFieldContext<IEnumerable<TKey>>, Task<ILookup<TKey, TReturn>>> _resolver;
        private readonly Func<TSource, TKey> _keySelector;

        public ChildBatchResolver(Func<ResolveFieldContext<IEnumerable<TKey>>, Task<ILookup<TKey, TReturn>>> resolver, Func<TSource, TKey> keySelector)
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

        public async Task<object> ResolveInternal(ResolveFieldContext context)
        {
            var key = _keySelector((TSource)context.Source);

            var tableKey = context.Document;
            Task<ILookup<TKey, TReturn>> task = null;
            if (!_resultsTable.TryGetValue(tableKey, out task))
            {
                var currentBatch = BatchStack.Peek();
                context.Source = BatchStack.Peek().OfType<TSource>().Select(_keySelector).Distinct();

                task = _resolver(new ResolveFieldContext<IEnumerable<TKey>>(context));
                _resultsTable.Add(tableKey, task);

                var newBatch = await task.ConfigureAwait(false);
                return BatchStack.Push(newBatch.SelectMany(x => x.ToList()), newBatch[key]);
            }

            return (await task.ConfigureAwait(false))[key];
        }
    }
}