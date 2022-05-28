using Sidecar.Model;

namespace Sidecar.Services
{
    public interface IDataAccess<T>
    {
        // Task<List<T>> QueryByZip(string zipcode);
        Task<QueryResult> QueryByZip(string zipcode, int max_retrieve, string? continuation_token);

        Task<QueryResult> QueryByState(string state, int max_retrieve, string? continuation_token);

        Task<QueryResult> QuerySql(string query, int max_retrieve, string? continuation_token);

        Task<GenericQueryResult> GenericQuerySql(string query, int max_retrieve, string? continuation_token);

    }
}
