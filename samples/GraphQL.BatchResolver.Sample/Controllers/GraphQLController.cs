using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.BatchResolver.Sample.Schema;
using GraphQL.Http;
using GraphQL.Instrumentation;
using Microsoft.AspNetCore.Mvc;

namespace GraphQL.BatchResolver.Sample
{
    [Route("api/graphql")]
    public class GraphQLController : Controller
    {
        private static int _queryNumber;
        private readonly IDocumentExecuter _executer;
        private readonly IDocumentWriter _writer;
        private readonly StarWarsSchema _schema;

        public GraphQLController(StarWarsSchema schema, IDocumentExecuter executer, IDocumentWriter writer)
        {
            _executer = executer;
            _schema = schema;
            _writer = writer;
        }

        [HttpPost]
        public async Task<ExecutionResult> Post([FromBody] GraphQLRequest request)
        {
            var queryNumber = Interlocked.Increment(ref _queryNumber);
            var start = DateTime.UtcNow;
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId.ToString().PadLeft(2, ' ')} - Running query #{queryNumber}");
            var sw = Stopwatch.StartNew();

            var result = await _executer.ExecuteAsync(_ =>
            {
                _.Query = request.Query;
                _.Schema = _schema;
                _.UserContext = new StarWarsContext();
                _.FieldMiddleware.Use<InstrumentFieldsMiddleware>();
            });

            sw.Stop();
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId.ToString().PadLeft(2, ' ')} - Finished query #{queryNumber} ({sw.ElapsedMilliseconds}ms, {result.Perf.Length} field resolutions)");

            return result;
        }
    }

    public class GraphQLRequest
    {
        public string Query { get; set; }
        public object Variables { get; set; }
    }
}