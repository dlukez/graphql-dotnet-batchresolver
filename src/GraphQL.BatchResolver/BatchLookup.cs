using System.Collections.Generic;
using System.Linq;

namespace GraphQL.BatchResolver
{
    internal static class BatchLookup
    {
        private static IDictionary<object, IEnumerable<object>> _lookup = new Dictionary<object, IEnumerable<object>>();

        public static IEnumerable<object> GetCollection(object value)
        {
            return _lookup[value];
        }

        public static void SaveCollection(IEnumerable<object> enumerable)
        {
            _lookup[enumerable.First()] = enumerable;
        }
    }
}