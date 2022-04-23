using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace WisdomPetMdicine.RescueQuery.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("[controller]")]
    public class RescueQueryController : ControllerBase
    {
        private readonly IConfiguration config;

        public RescueQueryController(IConfiguration config)
        {
            
            this.config = config;
        }

        [HttpGet]
        //https://localhost:44386/rescuequery?api-version=2.0
        public async Task<IActionResult> Get()
        {
            string sql = @"select * from adopters";
           using var conection= new SqlConnection(this.config.GetConnectionString("Rescue"));
            var data = (await conection.QueryAsync(sql)).ToList();
            return Ok(data);


        }
    }
}