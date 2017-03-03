using System.Linq;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;

namespace GraphQL.BatchResolver.Sample.Schema
{
    public class EpisodeType : ObjectGraphType<Episode>
    {
        public EpisodeType()
        {
            Name = "Episode";

            Field("id", e => e.EpisodeId);
            Field("name", e => e.Name);

            Field<ListGraphType<CharacterInterface>>()
                .Name("characters")
                .Batch(e => e.EpisodeId)
                .Resolve(async ctx =>
                {
                    var ids = ctx.Source;
                    var db = ctx.GetDataContext();

                    var humans = await db.HumanAppearances
                        .Where(ha => ids.Contains(ha.EpisodeId))
                        .Select(ha => new HumanAppearance { EpisodeId = ha.EpisodeId, Human = ha.Human })
                        .ToListAsync<ICharacterAppearance>();

                    var droids = await db.DroidAppearances
                        .Where(da => ids.Contains(da.EpisodeId))
                        .Select(da => new DroidAppearance { EpisodeId = da.EpisodeId, Droid = da.Droid })
                        .ToListAsync<ICharacterAppearance>();

                    return humans.Concat(droids).ToLookup(a => a.EpisodeId, a => a.Character);
                });
        }
    }

    public class EpisodeEnumType : EnumerationGraphType
    {
        public EpisodeEnumType()
        {
            Name = "EpisodeEnum";
            Description = "One of the films in the Star Wars Trilogy.";
            AddValue("NEWHOPE", "Released in 1977.", 4);
            AddValue("EMPIRE", "Released in 1980.", 5);
            AddValue("JEDI", "Released in 1983.", 6);
        }
    }

    public enum Episodes
    {
        NEWHOPE  = 4,
        EMPIRE  = 5,
        JEDI  = 6
    }
}