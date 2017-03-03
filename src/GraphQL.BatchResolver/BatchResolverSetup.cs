using System.Reflection;
using GraphQL.Instrumentation;
using GraphQL.Types;
using Type = System.Type;

namespace GraphQL.BatchResolver
{
    public static class BatchResolverSetup
    {
        private const string MarkerMetadataId = nameof(GraphQL.BatchResolver.BatchResolverSetup);

        /// <summary>
        /// Sets up support for BatchResolver.
        /// </summary>
        public static void WithBatching(this ExecutionOptions options)
        {
            options.FieldMiddleware.Use<BatchingMiddleware>();
            options.Schema.AllTypes.Apply(ConfigureType);
        }

        /// <summary>
        /// Prepares a GraphQL schema to support GraphNode/NodeCollection.
        /// </summary>
        /// <remarks>
        /// This method goes through each object type in the schema and ensures <see cref="GraphNode{T}" />
        /// is considered a valid equivalent for an <code>ObjectGraphType{T}</code>. Specifically,
        /// it replaces the <code>IsTypeOf</code> property with a wrapped version that checks if the
        /// given object is a compatible <see cref="GraphNode{T}"/>.
        /// </remarks>
        /// <seealso cref="NodeCollection{T}"/>
        private static void ConfigureSchema(this ISchema schema)
        {
            schema.AllTypes.Apply(ConfigureType);
        }

        /// <summary>
        /// Adds support for GraphNode{T} to an ObjectGraphType.
        /// </summary>
        /// <param name="type"></param>
        private static void ConfigureType(this IGraphType type)
        {
            // For an `ObjectGraphType`...
            var objectType = type as IObjectGraphType;
            if (objectType == null) return;

            // if we haven't already...
            if (objectType.HasMetadata(MarkerMetadataId)) return;

            // with the `IsTypeOf` property set...
            var isTypeOf = objectType.IsTypeOf;
            if (isTypeOf == null) return;

            // and an underlying type `T`...
            var innerType = FindObjectType(objectType.GetType());
            if (innerType == null) return;

            // replace the `IsTypeOf` property to support `GraphNode<T>`.
            var nodeType = typeof(GraphNode<>).MakeGenericType(innerType);
            objectType.IsTypeOf = value => isTypeOf((value as IGraphNode)?.Value ?? value);
            objectType.Metadata[MarkerMetadataId] = true;
        }

        private static Type FindObjectType(Type type)
        {
            while (type != null)
            {
                if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(ObjectGraphType<>))
                    return type.GenericTypeArguments[0];
                type = type.GetTypeInfo().BaseType;
            }
            return null;
        }
    }
}