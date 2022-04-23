using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace WisdomPetMdicine.RescueQuery.Api.Controllers.V2
{
    [ApiVersion("2.0")]
    [ApiController]
    [Route("[controller]")]
    public class RescueQueryController : ControllerBase
    {
        private readonly IConfiguration config;

        public RescueQueryController(IConfiguration config)
        {
            
            this.config = config;
        }
        //https://localhost:44386/rescuequery?api-version=2.0
        [HttpGet]
        public async Task<IActionResult> Get()
        {
          //  string sql = @"select * from adopters";
          // using var conection= new SqlConnection(this.config.GetConnectionString("Rescue"));
          //  var data = (await conection.QueryAsync(sql)).ToList();
            
            return Ok("prueba");


        }
        [HttpGet("prueba")]
        public async Task<IActionResult> prueba()
        {
            return Ok("version 2");
        }
    }
}