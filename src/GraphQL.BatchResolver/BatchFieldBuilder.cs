using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Builders;
using GraphQL.Types;

namespace GraphQL.BatchResolver
{
    public static class FieldBuilderExtensions
    {
        public static BatchFieldBuilder<TSource> Batch<TSource>(this FieldBuilder<TSource, object> builder)
        {
            return new BatchFieldBuilder<TSource>(builder.FieldType);
        }
        public static BatchFieldBuilder<TSource, TKey> Batch<TSource, TKey>(this FieldBuilder<TSource, object> builder, Func<TSource, TKey> keySelector)
        {
            return new BatchFieldBuilder<TSource, TKey>(builder.FieldType, keySelector);
        }
    }

    public class BatchFieldBuilder<TSource> : BatchFieldBuilder<TSource, TSource>
    {
        public BatchFieldBuilder(FieldType fieldType)
            : base(fieldType, x => x)
        {
        }
    }

    public class BatchFieldBuilder<TSource, TKey>
    {
        private readonly FieldType _field;
        private readonly Func<TSource, TKey> _keySelector;

        public BatchFieldBuilder(FieldType field, Func<TSource, TKey> keySelector)
        {
            _field = field;
            _field.Resolver = null;
            _keySelector = keySelector;
        }
        
        public void Resolve<TReturn>(Func<ResolveFieldContext<IEnumerable<TKey>>, Task<ILookup<TKey, TReturn>>> fetch)
        {
            _field.Resolver = new BatchFieldResolver<TSource, TKey, TReturn>(fetch, _keySelector);
        }
    }
}