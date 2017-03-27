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
                .ResolveMany(e => e.EpisodeId, ctx =>
                {
                    var ids = ctx.Source;
                    var db = ctx.GetDataContext();

                    var humanAppearances = db.HumanAppearances
                        .Where(ha => ids.Contains(ha.EpisodeId))
                        .Include(ha => ha.Human)
                        .ToList<ICharacterAppearance>();

                    var droidAppearances = db.DroidAppearances
                        .Where(da => ids.Contains(da.EpisodeId))
                        .Include(da => da.Droid)
                        .ToList<ICharacterAppearance>();

                    return humanAppearances.Concat(droidAppearances).ToLookup(a => a.EpisodeId, a => a.Character);
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