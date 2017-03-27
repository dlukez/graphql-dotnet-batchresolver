using System;
using System.Linq;

namespace GraphQL.BatchResolver.Sample
{
    public class TestDataOptions
    {
        public int Seed { get; set; } = 42;
        public int NumberOfHumans { get; set; } = 200;
        public int NumberOfDroids { get; set; } = 200;
        public int NumberOfFriendships { get; set; } = 500;
        public int NumberOfAppearances { get; set; } = 2;
    }

    public class TestDataGenerator
    {
        public static void InitializeDb() => InitializeDb(new TestDataOptions());

        public static void InitializeDb(TestDataOptions options)
        {
            using (var db = new StarWarsContext())
            {
                db.Droids.RemoveRange(db.Droids);
                db.Humans.RemoveRange(db.Humans);
                db.Episodes.RemoveRange(db.Episodes);
                db.SaveChanges();

                // Generation
                var random = new Random(options.Seed);

                // Episodes
                db.Episodes.AddRange(new[]
                {
                    new Episode { EpisodeId = 4, Name = "A New Hope", Year = "1978" },
                    new Episode { EpisodeId = 5, Name = "Rise of the Empire", Year = "1980" },
                    new Episode { EpisodeId = 6, Name = "Return of the Jedi", Year = "1983" }
                });

                // Humans
                db.Humans.AddRange(
                    Enumerable.Range(1, options.NumberOfHumans).Select(id => new Human
                    {
                        HumanId = id,
                        Name = Faker.Name.First(),
                        HomePlanet = Faker.Company.Name(),
                        Appearances = Enumerable.Repeat(1, options.NumberOfAppearances).Select(_ => new HumanAppearance
                        {
                            EpisodeId = random.Next(4, 7),
                            HumanId = id
                        }).ToList()
                    }));

                // Droids
                db.Droids.AddRange(
                    Enumerable.Range(1, options.NumberOfDroids).Select(id => new Droid
                    {
                        DroidId = id,
                        Name = $"{(char)random.Next('A', 'Z')}"
                             +       $"{random.Next(1, 9)}"
                             + $"{(char)random.Next('A', 'Z')}"
                             +       $"{random.Next(1, 9)}",
                        PrimaryFunction = Faker.Company.BS(),
                        Appearances = Enumerable.Range(1, options.NumberOfAppearances).Select(_ => new DroidAppearance
                        {
                            EpisodeId = random.Next(4, 7),
                            DroidId = random.Next(1, options.NumberOfDroids)
                        }).ToList()
                    }));


                // Friendships
                db.Friendships.AddRange(
                    Enumerable.Range(1, options.NumberOfFriendships).Select(_ => new Friendship
                    {
                        DroidId = random.Next(1, options.NumberOfDroids),
                        HumanId = random.Next(1, options.NumberOfHumans)
                    }));

                // Save
                db.SaveChanges();
            }
        }
    }
}
