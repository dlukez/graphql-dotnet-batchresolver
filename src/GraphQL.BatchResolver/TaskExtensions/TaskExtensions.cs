using System.Collections.Generic;
using System.Threading.Tasks;

namespace GraphQL.BatchResolver.TaskExtensions
{
    public static class TaskEnumerableExtensions
    {
        public static Task<IEnumerable<T>> AsEnumerable<T>(this Task<List<T>> listTask)
        {
            return listTask.ContinueWith(t => (IEnumerable<T>)t.Result, TaskContinuationOptions.ExecuteSynchronously);
        }

        public static Task<IEnumerable<T>> AsEnumerable<T>(this Task<T[]> arrayTask)
        {
            return arrayTask.ContinueWith(t => (IEnumerable<T>)t.Result, TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}