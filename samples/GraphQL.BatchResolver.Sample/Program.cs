using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;

namespace GraphQL.BatchResolver.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            InitTestData();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseIISIntegration()
                .Build();

            host.Run();
        }

        private static void InitTestData()
        {
            using (var db = new StarWarsContext())
            {
                if (db.Humans.Any()) return;

                // Clear
                db.Humans.RemoveRange(db.Humans);
                db.Droids.RemoveRange(db.Droids);
                db.Episodes.RemoveRange(db.Episodes);
                db.Friendships.RemoveRange(db.Friendships);
                db.DroidAppearances.RemoveRange(db.DroidAppearances);
                db.HumanAppearances.RemoveRange(db.HumanAppearances);
                db.SaveChanges();

                // Regenerate
                const int numberOfHumans = 1000;
                const int numberOfDroids = 1000;
                const int numberOfFriendships = 1500;
                const int numberOfAppearances = 2;
                var random = new Random();
                
                // Episodes
                db.Episodes.AddRange(new[]
                {
                    new Episode { EpisodeId = 4, Name = "A New Hope", Year = "1978" },
                    new Episode { EpisodeId = 5, Name = "Rise of the Empire", Year = "1980" },
                    new Episode { EpisodeId = 6, Name = "Return of the Jedi", Year = "1983" }
                });

                // Humans
                db.Humans.AddRange(
                    Enumerable.Range(1, numberOfHumans).Select(id => new Human
                    {
                        HumanId = id,
                        Name = Faker.Name.First(),
                        HomePlanet = Faker.Company.Name(),
                        Appearances = Enumerable.Repeat(1, numberOfAppearances).Select(_ => new HumanAppearance
                            {
                                EpisodeId = random.Next(4, 6),
                                HumanId = id
                            }).ToList()
                    }));

                // Droids
                db.Droids.AddRange(
                    Enumerable.Range(1, numberOfDroids).Select(id => new Droid
                    {
                        DroidId = id,
                        Name = $"{(char)random.Next('A', 'Z')}"
                            + $"{random.Next(1, 9)}"
                            + $"{(char)random.Next('A', 'Z')}"
                            + $"{random.Next(1, 9)}",
                        PrimaryFunction = Faker.Company.BS(),
                        Appearances = Enumerable.Range(1, numberOfAppearances).Select(_ => new DroidAppearance
                            {
                                EpisodeId = random.Next(4, 6),
                                DroidId = random.Next(1, numberOfDroids)
                            }).ToList()
                    }));


                // Friendships
                db.Friendships.AddRange(
                    Enumerable.Range(1, numberOfFriendships).Select(_ => new Friendship
                    {
                        DroidId = random.Next(1, numberOfDroids),
                        HumanId = random.Next(1, numberOfHumans)
                    }));

                // Save
                var count = db.SaveChanges();
                Console.WriteLine("{0} records saved to database", count);
            }
        }
    }
}