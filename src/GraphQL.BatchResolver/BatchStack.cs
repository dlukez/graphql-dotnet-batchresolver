using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphQL.BatchResolver
{
    public static class BatchStack
    {
        [ThreadStatic]
        private static Stack<IEnumerable<object>> _threadStack;

        private static Stack<IEnumerable<object>> CurrentStack
        {
            get
            { 
                if (_threadStack == null)
                    _threadStack = new Stack<IEnumerable<object>>();

                return _threadStack;
            }
        }

        public static int Count => CurrentStack.Count;

        public static IEnumerable<TResult> Push<TResult>(IEnumerable<TResult> result)
        {
            return Push(result, result);
        }

        public static IEnumerable<TResult> Push<TBatch, TResult>(IEnumerable<TBatch> batch, IEnumerable<TResult> actualResult)
        {
            CurrentStack.Push((IEnumerable<object>)batch);
            foreach (var item in actualResult)
            {
                yield return item;
            }
            CurrentStack.Pop();
        }

        public static IEnumerable<object> Peek() => CurrentStack.Peek();

        public static IEnumerable<object> Pop() => CurrentStack.Pop();

        public static string Dump()
        {
            var num = 0;
            var str = "---> ";
            str += string.Join("     ", CurrentStack.Select(s => $"{++num}: Enumerable of {s.GetType().GenericTypeArguments[0].Name} ({s.Count()} items)" + Environment.NewLine));
            return str;
        }
    }
}