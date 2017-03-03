/*
 *  Example 1: One loader instance for each HumanType instance.
 *
 *    If the schema is created once on application startup and reused
 *    for every request, then the same loader will be used by
 *    multiple requests/threads. This is probably unsafe.
 */

using System.Linq;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;

namespace GraphQL.BatchResolver.Sample.Schema
{
    public class DroidType : ObjectGraphType<Droid>
    {
        public DroidType()
        {
            Name = "Droid";
            Field(d => d.Name);
            Field(d => d.DroidId);
            Field(d => d.PrimaryFunction);

            Field<ListGraphType<CharacterInterface>>()
                .Name("friends")
                .Batch(d => d.DroidId)
                .Resolve(async ctx =>
                {
                    var ids = ctx.Source;
                    var db = ctx.GetDataContext();
                    return (await db.Friendships
                            .Where(f => ids.Contains(f.DroidId))
                            .Select(f => new { Key = f.DroidId, f.Human })
                            .ToListAsync())
                        .ToLookup(x => x.Key, x => (ICharacter)x.Human);
                });

            Field<ListGraphType<EpisodeType>>()
                .Name("appearsIn")
                .Batch(d => d.DroidId)
                .Resolve(async ctx =>
                {
                    var ids = ctx.Source;
                    var db = ctx.GetDataContext();
                    return (await db.DroidAppearances
                            .Where(da => ids.Contains(da.DroidId))
                            .Select(da => new { Key = da.DroidId, da.Episode })
                            .ToListAsync())
                        .ToLookup(x => x.Key, x => x.Episode);
                });
                
            Interface<CharacterInterface>();
        }
    }
}
