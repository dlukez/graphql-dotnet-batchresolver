using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GraphQL.BatchResolver
{
    public class NodeCollection<T> : IEnumerable<GraphNode<T>>
    {
        private readonly IList<T> _items;

        public NodeCollection(IList<T> list)
        {
            _items = list;
        }

        public NodeCollection(IEnumerable<T> enumerable)
        {
            _items = enumerable.ToList();
        }

        public IEnumerator<GraphNode<T>> GetEnumerator()
        {
            foreach (var value in _items)
                yield return new GraphNode<T>(value, _items);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}