namespace GraphQL.BatchResolver.Sample.Schema
{
    public class StarWarsSchema : global::GraphQL.Types.Schema
    {
        public StarWarsSchema()
        {
            Query = new StarWarsQuery();
        }
    }
}