using System.Collections.Generic;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;

namespace GraphQL.BatchResolver.Sample
{
    public static class ResolveFieldContextExtensions
    {
        public static StarWarsContext GetDataContext<T>(this ResolveFieldContext<T> context)
        {
            return (StarWarsContext)context.UserContext;
        }
    }
    
    public class StarWarsContext : DbContext
    {
        public DbSet<Human> Humans { get; set; }
        public DbSet<Droid> Droids { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<DroidAppearance> DroidAppearances { get; set; }
        public DbSet<HumanAppearance> HumanAppearances { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./starwars.db");
        }
    }

    public interface INode
    {
        int Id { get; }
    }

    public interface ICharacter
    {
        string Name { get; set; }
        List<Friendship> Friendships { get; set; }
    }

    public class Human : ICharacter, INode
    {
        int INode.Id => HumanId;
        public int HumanId { get; set; }
        public string Name { get; set; }
        public string HomePlanet { get; set; }
        public List<Friendship> Friendships { get; set; }
        public List<HumanAppearance> Appearances { get; set; }

        public override string ToString()
        {
            return HumanId.ToString();
        }
    }

    public class Friendship
    {
        public int FriendshipId { get; set; }
        public int HumanId { get; set; }
        public int DroidId { get; set; }
        public Human Human { get; set; }
        public Droid Droid { get; set; }
    }

    public class Droid : ICharacter, INode
    {
        int INode.Id => DroidId;
        public int DroidId { get; set; }
        public string Name { get; set; }
        public string PrimaryFunction { get; set; }
        public List<Friendship> Friendships { get; set; }
        public List<DroidAppearance> Appearances { get; set; }

        public override string ToString()
        {
            return DroidId.ToString();
        } 
    }

    public interface ICharacterAppearance
    {
        int EpisodeId { get; }
        Episode Episode { get; }
        int CharacterId { get; }
        ICharacter Character { get; }
    }

    public class DroidAppearance : ICharacterAppearance, INode
    {
        int INode.Id => DroidAppearanceId;
        public int DroidAppearanceId { get; set; }
        public int EpisodeId { get; set; }
        public Episode Episode { get; set; }
        public int DroidId { get; set; }
        public Droid Droid { get; set; }
        int ICharacterAppearance.CharacterId => DroidId;
        ICharacter ICharacterAppearance.Character => Droid;
    }

    public class HumanAppearance : ICharacterAppearance, INode
    {
        int INode.Id => HumanAppearanceId;
        public int HumanAppearanceId { get; set; }
        public int EpisodeId { get; set; }
        public Episode Episode { get; set; }
        public int HumanId { get; set; }
        public Human Human { get; set; }
        int ICharacterAppearance.CharacterId => HumanId;
        ICharacter ICharacterAppearance.Character => Human;
    }

    public class Episode : INode
    {
        int INode.Id => EpisodeId;
        public int EpisodeId { get; set; }
        public string Name { get; set; }
        public string Year { get; set; }
        public List<DroidAppearance> DroidAppearances { get; set; }
        public List<HumanAppearance> HumanAppearances { get; set; }
    }
}
