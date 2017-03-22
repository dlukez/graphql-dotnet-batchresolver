using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace GraphQL.BatchResolver
{
    internal static class BatchStack
    {
        private static Stack<IEnumerable<object>> _stack = new Stack<IEnumerable<object>>();

        public static int Count => _stack.Count;

        public static IEnumerable<TResult> Push<TResult>(IEnumerable<TResult> result)
        {
            return Push(result, result);
        }

        public static IEnumerable<TResult> Push<TBatch, TResult>(IEnumerable<TBatch> batch, IEnumerable<TResult> actualResult)
        {
            _stack.Push((IEnumerable<object>)batch);
            foreach (var item in actualResult)
            {
                yield return item;
            }
            _stack.Pop();
        }

        public static IEnumerable<object> Peek() => _stack.Peek();

        public static IEnumerable<object> Pop() => _stack.Pop();
    }
}