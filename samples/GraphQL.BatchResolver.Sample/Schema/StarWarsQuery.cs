using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.BatchResolver;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;

namespace GraphQL.BatchResolver.Sample.Schema
{
    public class StarWarsQuery : ObjectGraphType
    {
        public StarWarsQuery()
        {
            Name = "Query";

            Field<ListGraphType<HumanType>>(
                "humans",
                resolve: ctx => ctx.GetDataContext().Humans.ToListAsync());

            Field<ListGraphType<DroidType>>(
                "droids",
                resolve: ctx => ctx.GetDataContext().Droids.ToListAsync());

            Field<ListGraphType<EpisodeType>>(
                "episodes",
                resolve: ctx => ctx.GetDataContext().Episodes.ToListAsync());
        }
    }
}
