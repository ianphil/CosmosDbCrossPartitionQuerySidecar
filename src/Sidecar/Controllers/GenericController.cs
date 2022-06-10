using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sidecar.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sidecar.Services;

namespace Sidecar.Controllers
{
    public class QueryParametersQuery
    {
        [BindRequired]
        public string connectionstring { get; set; }
        [BindRequired]
        public string sql { get; set; }
        public string? ctoken { get; set; }
        public int? limit { get; set; }

        public QueryParametersQuery(){
            this.connectionstring = String.Empty;
            this.sql = String.Empty;
        }
    }


    [Route("api/[controller]")]
    [ApiController]
    public class GenericController : Controller
    {
        #region Privates/Constants per discussion
        /// <summary>
        /// Constants as requested for database ID and container ID. MAX_RETRIEVE is 
        /// currently set artifically low so we can see continuation tokens working.
        /// </summary>
        private const string DATABASE_ID = "addressesdb";
        private const string CONTAINER_ID = "address";
        private const int MAX_RETRIEVE = 2;
        #endregion

        private readonly IDataAccess<Address> _dataAccess;

        /// <summary>
        /// In mem cache for access objects, but likely not the right solution long term.
        /// </summary>
        private static Dictionary<string, IDataAccess<Address>>? _access_map;

        public GenericController(IDataAccess<Address> dataAccess)
        {
            _dataAccess = dataAccess;

            if (GenericController._access_map == null)
            {
                GenericController._access_map = new Dictionary<string, IDataAccess<Address>>();
            }
        }

        // GET api/<ZipController1>/30542
        [HttpGet("v1")]
        public async Task<string> Get([FromQuery] QueryParametersQuery parameters)
        {

            // If we don't have a IDataAccess instance for this connection string, 
            // create one. 
            if (!GenericController._access_map.ContainsKey(parameters.connectionstring))
            {
                GenericController._access_map.Add(
                    parameters.connectionstring,
                    new Cosmos(
                        parameters.connectionstring,
                        GenericController.DATABASE_ID,
                        GenericController.CONTAINER_ID)
                    );
            }

            // Get the IDataAccess instance for this connection string
            var access = GenericController._access_map[parameters.connectionstring];

            // Clean up token which is likely just the returned string from the last
            // call and may contain errant \\ characters. 
            if (parameters.ctoken != null)
            {
                parameters.ctoken = parameters.ctoken.Replace("\\", "");
            }

            int limit = GenericController.MAX_RETRIEVE;
            if (parameters.limit != null)
            {
                limit = parameters.limit.Value;
            }

            // Finally, query with a max number to get and the optional incoming
            // continuation token from the last call. 
            var data = await access.GenericQuerySql(
                parameters.sql,
                GenericController.MAX_RETRIEVE,
                parameters.ctoken
                );

            return JsonConvert.SerializeObject(data);
        }

    }
}
