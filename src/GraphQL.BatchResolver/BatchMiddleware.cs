using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Instrumentation;
using GraphQL.Types;

namespace GraphQL.BatchResolver
{
    /// <summary>
    /// Adds support for resolving collections in a batch.
    /// </summary>
    public class BatchMiddleware
    {
        public async Task<object> Resolve(ResolveFieldContext context, FieldMiddlewareDelegate next)
        {
            var result = await next(context).ConfigureAwait(false);

            var enumerable = result as IEnumerable<object>;
            if (enumerable != null && !(result is string) && enumerable.Any())
                return new BatchEnumerable(enumerable);

            return result;
        }
    }
}