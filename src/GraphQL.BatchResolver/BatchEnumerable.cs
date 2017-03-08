using System.Collections;
using System.Collections.Generic;

namespace GraphQL.BatchResolver
{
    public class BatchEnumerable : IEnumerable<object>
    {
        private IEnumerable<object> _enumerable;
        
        public BatchEnumerable(IEnumerable<object> enumerable)
        {
            _enumerable = enumerable;
        }

        public IEnumerator<object> GetEnumerator()
        {
            BatchStack.Push(_enumerable);
            foreach (var item in _enumerable)
            {
                yield return item;
            }
            BatchStack.Pop();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}