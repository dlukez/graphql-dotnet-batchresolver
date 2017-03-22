using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;

namespace GraphQL.BatchResolver.Sample.Schema
{
    public class StarWarsQuery : ObjectGraphType
    {
        public StarWarsQuery()
        {
            Name = "Query";

            Field<ListGraphType<HumanType>>()
                .Name("humans")
                .Batch()
                .Resolve(ctx => ctx.GetDataContext().Humans.ToListAsync().ContinueWith(t => (IEnumerable<Human>)t.Result, TaskContinuationOptions.ExecuteSynchronously));

            Field<ListGraphType<DroidType>>()
                .Name("droids")
                .Batch()
                .Resolve(ctx => ctx.GetDataContext().Droids.ToListAsync().ContinueWith(t => (IEnumerable<Droid>)t.Result, TaskContinuationOptions.ExecuteSynchronously));

            Field<ListGraphType<EpisodeType>>()
                .Name("episodes")
                .Batch()
                .Resolve(ctx => ctx.GetDataContext().Episodes.ToListAsync().ContinueWith(t => (IEnumerable<Episode>)t.Result, TaskContinuationOptions.ExecuteSynchronously));
        }
    }
}
