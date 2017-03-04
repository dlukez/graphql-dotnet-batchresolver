using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GraphQL.Resolvers;
using GraphQL.Types;
using GraphQL.Language.AST;

namespace GraphQL.BatchResolver
{
    public class BatchFieldResolver<TSource, TKey, TReturn> : IFieldResolver
    {
        private readonly ConditionalWeakTable<Field, Task<ILookup<TKey, TReturn>>> _table = new ConditionalWeakTable<Field, Task<ILookup<TKey, TReturn>>>();
        private readonly Func<ResolveFieldContext<IEnumerable<TKey>>, Task<ILookup<TKey, TReturn>>> _resolver;
        private readonly Func<TSource, TKey> _keySelector;

        public BatchFieldResolver(Func<TSource, TKey> keySelector, Func<ResolveFieldContext<IEnumerable<TKey>>, Task<ILookup<TKey, TReturn>>> resolver)
        {
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));
            _resolver = resolver;
            _keySelector = keySelector;
        }

        public object Resolve(ResolveFieldContext context)
        {
            return ResolveInternal(context);
        }

        public async Task<object> ResolveInternal(ResolveFieldContext context)
        {
            var task = _table.GetValue(context.FieldAst, _ => ExecuteFieldResolver(context));
            var data = await task.ConfigureAwait(false);
            return data[_keySelector((TSource)context.Source)];
        }

        private Task<ILookup<TKey, TReturn>> ExecuteFieldResolver(ResolveFieldContext context)
        {
            // Temporarily swap the source object so that we only have to
            // construct one new context object and to save us copying all
            // the properties individually.
            var source = context.Source;
            context.Source = BatchLookup.GetCollection(source).OfType<TSource>().Select(_keySelector);
            var typedContext = new ResolveFieldContext<IEnumerable<TKey>>(context);
            context.Source = source;

            return _resolver(typedContext);
        }
    }
}