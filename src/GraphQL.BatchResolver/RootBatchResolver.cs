using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace GraphQL.BatchResolver
{
    /// <summary>
    /// Adds support for resolving collections in a batch.
    /// </summary>
    public class RootBatchResolver<TReturn> : IFieldResolver
    {
        private readonly Func<ResolveFieldContext, Task<IEnumerable<TReturn>>> _resolve;

        public RootBatchResolver(Func<ResolveFieldContext, Task<IEnumerable<TReturn>>> resolve)
        {
            _resolve = resolve;
        }

        public Task<IEnumerable<TReturn>> Resolve(ResolveFieldContext context)
        {
            return _resolve(context).ContinueWith(t => BatchStack.Push(t.Result), TaskContinuationOptions.ExecuteSynchronously);
        }

        object IFieldResolver.Resolve(ResolveFieldContext context)
        {
            return Resolve(context);
        }
    }
}