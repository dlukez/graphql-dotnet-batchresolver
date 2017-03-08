using System.Collections.Generic;

namespace GraphQL.BatchResolver
{
    internal static class BatchStack
    {
        private static Stack<IEnumerable<object>> _stack = new Stack<IEnumerable<object>>();

        public static IEnumerable<object> Pop() => _stack.Pop();

        public static IEnumerable<object> Peek() => _stack.Peek();

        public static void Push(IEnumerable<object> enumerable) => _stack.Push(enumerable);
    }
}