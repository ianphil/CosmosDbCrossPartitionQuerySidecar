namespace Sidecar.Model
{
    public class QueryResult
    {
        public string? ContinuationToken { get; set; }
        public List<Address> Results { get; set; }

        public QueryResult()
        {
            this.Results = new List<Address>();
        }
    }

    public class GenericQueryResult
    {
        public string? ContinuationToken { get; set; }
        public List<Dictionary<string,string>> Results { get; set; }

        public GenericQueryResult()
        {
            this.Results = new List<Dictionary<string, string>>();
        }
    }

}
