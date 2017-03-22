using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace GraphQL.BatchResolver.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            TestDataGenerator.InitializeDb();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseIISIntegration()
                .Build();

            host.Run();
        }
    }
}