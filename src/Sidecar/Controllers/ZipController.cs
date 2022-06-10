using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Sidecar.Model;
using Sidecar.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sidecar.Controllers
{
    public class QueryParametersGet
    {
        [BindRequired]
        public string connectionstring { get; set; }
        public string? ctoken { get; set; }
        public int? limit { get; set; }

        public QueryParametersGet(){
            this.connectionstring = String.Empty;
        }
    }


    [Route("api/[controller]")]
    [ApiController]
    public class ZipController : ControllerBase
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

        public ZipController(IDataAccess<Address> dataAccess)
        {
            _dataAccess = dataAccess;

            if (ZipController._access_map == null)
            {
                ZipController._access_map = new Dictionary<string, IDataAccess<Address>>();
            }
        }

        // GET api/<ZipController1>/vi/30542
        [HttpGet("{zipCode}/v1")]
        public async Task<string> Get(string zipCode)
        {
            var data = await _dataAccess.QueryByZip(zipCode, 1000, null);
            return JsonConvert.SerializeObject(data);
        }

        // GET api/<ZipController1>/30542
        [HttpGet("{zipCode}/v2")]
        public async Task<string> Get(string zipCode, [FromQuery] QueryParametersGet parameters)
        {

            // If we don't have a IDataAccess instance for this connection string, 
            // create one. 
            if ( !ZipController._access_map.ContainsKey(parameters.connectionstring) )
            {
                ZipController._access_map.Add(
                    parameters.connectionstring, 
                    new Cosmos(
                        parameters.connectionstring, 
                        ZipController.DATABASE_ID, 
                        ZipController.CONTAINER_ID)
                    );
            }

            // Get the IDataAccess instance for this connection string
            var access = ZipController._access_map[parameters.connectionstring];

            // Clean up token which is likely just the returned string from the last
            // call and may contain errant \\ characters. 
            if( parameters.ctoken != null)
            {
                parameters.ctoken = parameters.ctoken.Replace("\\", "");
            }

            int limit = ZipController.MAX_RETRIEVE;
            if ( parameters.limit != null)
            {
                limit = parameters.limit.Value;
            }

            // Finally, query with a max number to get and the optional incoming
            // continuation token from the last call. 
            var data = await access.QueryByZip(
                zipCode, 
                ZipController.MAX_RETRIEVE, 
                parameters.ctoken
                );

            return JsonConvert.SerializeObject(data);
        }

    }
}
