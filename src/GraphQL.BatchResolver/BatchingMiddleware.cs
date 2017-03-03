using System.Threading.Tasks;
using GraphQL.Instrumentation;
using GraphQL.Types;

namespace GraphQL.BatchResolver
{
    public class BatchingMiddleware
    {
        public Task<object> Resolve(ResolveFieldContext context, FieldMiddlewareDelegate next)
        {
            if (!context.FieldDefinition.HasMetadata(MetadataKeys.FieldIsBatched))
            {
                var node = context.Source as IGraphNode;
                if (node != null)
                    context.Source = node.Value;
            }

            return next(context);
        }
    }
}