using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GraphQL.Resolvers;
using GraphQL.Types;
using GraphQL.Language.AST;
using System.Collections;

namespace GraphQL.BatchResolver
{
    public class BatchFieldResolver<TSource, TKey, TReturn> : IFieldResolver
    {
        private readonly ConditionalWeakTable<Field, Task<ILookup<TKey, TReturn>>> _resultsTable = new ConditionalWeakTable<Field, Task<ILookup<TKey, TReturn>>>();
        private readonly Func<ResolveFieldContext<IEnumerable<TKey>>, Task<ILookup<TKey, TReturn>>> _resolver;
        private readonly Func<TSource, TKey> _keySelector;

        public BatchFieldResolver(Func<ResolveFieldContext<IEnumerable<TKey>>, Task<ILookup<TKey, TReturn>>> resolver, Func<TSource, TKey> keySelector)
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
            var source = context.Source;
            var data = await _resultsTable.GetValue(context.FieldAst, _ => 
            {
                context.Source = BatchStack.Peek().OfType<TSource>().Select(_keySelector);
                var typedContext = new ResolveFieldContext<IEnumerable<TKey>>(context);
                return _resolver(typedContext);
            }).ConfigureAwait(false);
            return data[_keySelector((TSource)source)];
        }
    }
}