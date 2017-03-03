/*
 *  Example 2: Each request has its own context passed through the UserContext.
 *
 *    This helps keep queries/batches separate and is more likely to prevent
 *    issues arising from other queries that are executing concurrently.
 */

using System.Linq;
using GraphQL.BatchResolver;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;

namespace GraphQL.BatchResolver.Sample.Schema
{
    public class HumanType : ObjectGraphType<Human>
    {
        public HumanType()
        {
            Name = "Human";
            Field(h => h.Name);
            Field(h => h.HumanId);
            Field(h => h.HomePlanet);

            Field<ListGraphType<CharacterInterface>>()
                .Name("friends")
                .Batch(h => h.HumanId)
                .Resolve(async ctx =>
                {
                    var ids = ctx.Source;
                    var db = ctx.GetDataContext();
                    return (await db.Friendships
                            .Where(f => ids.Contains(f.HumanId))
                            .Select(f => new {Key = f.HumanId, f.Droid})
                            .ToListAsync())
                        .ToLookup(x => x.Key, x => (ICharacter)x.Droid);
                });

            Field<ListGraphType<EpisodeType>>()
                .Name("appearsIn")
                .Batch(h => h.HumanId)
                .Resolve(async ctx =>
                {
                    var ids = ctx.Source;
                    var db = ctx.GetDataContext();
                    return (await db.HumanAppearances
                            .Where(ha => ids.Contains(ha.HumanId))
                            .Select(ha => new { Key = ha.HumanId, ha.Episode })
                            .ToListAsync())
                        .ToLookup(x => x.Key, x => x.Episode);
                });

            Interface<CharacterInterface>();
        }
    }
}
