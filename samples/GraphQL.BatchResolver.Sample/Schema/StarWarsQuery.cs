using System.Linq;
using GraphQL.Types;

namespace GraphQL.BatchResolver.Sample.Schema
{
    public class StarWarsQuery : ObjectGraphType
    {
        public StarWarsQuery()
        {
            Name = "Query";

            Field<ListGraphType<HumanType>>()
                .Name("humans")
                .Resolve(ctx => ctx.GetDataContext().Humans.ToList());

            Field<ListGraphType<DroidType>>()
                .Name("droids")
                .Resolve(ctx => ctx.GetDataContext().Droids.ToList());

            Field<ListGraphType<EpisodeType>>()
                .Name("episodes")
                .Resolve(ctx => ctx.GetDataContext().Episodes.ToList());
        }
    }
}
