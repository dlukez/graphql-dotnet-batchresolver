using System.Collections;
using System.Collections.Generic;

namespace GraphQL.BatchResolver
{
    public interface IGraphNode
    {
        IEnumerable Collection { get; }
        object Value { get; }
    }
    
    public class GraphNode<T> : IGraphNode
    {
        public T Value { get; }
        public IEnumerable<T> Collection { get; }
        object IGraphNode.Value => Value;
        IEnumerable IGraphNode.Collection => Collection;

        public GraphNode(T value, IEnumerable<T> enumerable)
        {
            Value = value;
            Collection = enumerable;
        }

        public static explicit operator T(GraphNode<T> node) => node.Value;
    }
}