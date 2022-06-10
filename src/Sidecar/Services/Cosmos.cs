using Microsoft.Azure.Cosmos;
using Sidecar.Model;
//using System.ComponentModel;

namespace Sidecar.Services
{


    public class Cosmos : IDataAccess<Address>
    {
        private readonly CosmosClient cosmosClient;
        private readonly Container container;
        private readonly Database database;

        public Cosmos()
        {
            string conn = Environment.GetEnvironmentVariable("ConnString");
            string databaseId = Environment.GetEnvironmentVariable("DatabaseId");
            string containerId = Environment.GetEnvironmentVariable("ContainerId");

            cosmosClient = new CosmosClient(conn);
            database = cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId).GetAwaiter().GetResult();
            container = database.CreateContainerIfNotExistsAsync(containerId, "/zipCode").GetAwaiter().GetResult();
        }

        public Cosmos(string connection_string, string dbid, string contid)
        {
            string conn = connection_string;
            string databaseId = dbid;
            string containerId = contid;

            cosmosClient = new CosmosClient(conn);
            database = cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId).GetAwaiter().GetResult();
            container = database.CreateContainerIfNotExistsAsync(containerId, "/zipCode").GetAwaiter().GetResult();
        }

        public async Task<QueryResult> QuerySql(string query, int max_retrieve, string? continuation_token)
        {
            return await this.Query(query, max_retrieve, continuation_token);
        }

        public async Task<GenericQueryResult> GenericQuerySql(string query, int max_retrieve, string? continuation_token)
        {
            // return await this.Query(query, max_retrieve, continuation_token);
            return await this.GenericQuery(query, max_retrieve, continuation_token);
        }

        public async Task<QueryResult> QueryByZip(string zipcode, int max_retrieve, string? continuation_token)
        {
            var sqlQueryText = $"SELECT * FROM c WHERE c.zipCode = '{zipcode}'";

            var result = await this.Query(sqlQueryText, max_retrieve, continuation_token);
            
            return result;
        }

        public async Task<QueryResult> QueryByState(string state, int max_retrieve, string? continuation_token)
        {
            var sqlQueryText = $"SELECT * FROM c WHERE c.state = '{state}'";

            var result = await this.Query(sqlQueryText, max_retrieve, continuation_token);

            return result;
        }

        /// <summary>
        /// Generice Cosmos query with SQL text
        /// </summary>
        /// <param name="sqlQueryText">SQL Query to execute</param>
        /// <param name="max_query_count">Max number of results to return</param>
        /// <param name="continuation_token">Optional continuation token</param>
        /// <returns>
        ///     QueryResult object with found results and optional continuation token.
        ///     Return token must be valid. See Zip/Generic controller to ensure it 
        ///     has been appropriately cleaned before being sent in. 
        /// </returns>
        private async Task<QueryResult> Query(string sqlQueryText, int max_query_count, string? continuation_token)
        {
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            QueryResult return_result = new QueryResult();

            using (FeedIterator<Address> queryResultSetIterator = this.container.GetItemQueryIterator<Address>(
                queryDefinition,
                requestOptions: new QueryRequestOptions()
                {
                    MaxItemCount = max_query_count
                },
                continuationToken: continuation_token))
            {
                if (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<Address> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    return_result.ContinuationToken = currentResultSet.ContinuationToken;

                    foreach (Address Address in currentResultSet)
                    {
                        return_result.Results.Add(Address);
                    }
                }
            }

            return return_result;
        }

        /// <summary>
        /// Generic query where return isn't pinned to a type but to a generic dictionary.
        /// </summary>
        /// <param name="sqlQueryText">SQL Query to execute</param>
        /// <param name="max_query_count">Max number of results to return</param>
        /// <param name="continuation_token">Optional continuation token</param>
        /// <returns>
        ///     GenericQueryResult object with found results and optional continuation token.
        ///     Return token must be valid. See Zip/Generic controller to ensure it 
        ///     has been appropriately cleaned before being sent in. 
        /// </returns>
        private async Task<GenericQueryResult> GenericQuery(string sqlQueryText, int max_query_count, string? continuation_token)
        {
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            GenericQueryResult return_result = new GenericQueryResult();

            using (FeedIterator<Dictionary<string,string> >queryResultSetIterator = this.container.GetItemQueryIterator<Dictionary<string, string>>(
                queryDefinition,
                requestOptions: new QueryRequestOptions()
                {
                    MaxItemCount = max_query_count
                },
                continuationToken: continuation_token))
            {
                if (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<Dictionary<string, string>> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    return_result.ContinuationToken = currentResultSet.ContinuationToken;

                    List<Dictionary<string, string>> res = new List<Dictionary<string, string>>();
                    foreach (Dictionary<string, string> record in currentResultSet)
                    {
                        return_result.Results.Add(record);
                    }
                }
            }

            return return_result;
        }

        /*
        public async Task<List<Address>> QueryByState(string state)
        {
            var sqlQueryText = $"SELECT * FROM c WHERE c.state = '{state}'";

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Address> queryResultSetIterator = this.container.GetItemQueryIterator<Address>(queryDefinition);

            List<Address> addresses = new List<Address>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Address> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (Address Address in currentResultSet)
                {
                    addresses.Add(Address);
                }
            }

            return addresses;
        }
        */
    }
}
