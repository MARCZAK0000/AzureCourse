using Azure.Data.Tables;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TableStorage.Entities;

namespace TableStorage.Controller
{
    [ApiController]
    [Route("/")]
    public class TableStorageController:ControllerBase
    {
        private readonly TableServiceClient _tableServiceClient;

        public TableStorageController(TableServiceClient tableServiceClient)
        {
            _tableServiceClient = tableServiceClient;
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get([FromQuery] string partKey, string rowKey) 
        {
            var tableName = "employee";

            var table = _tableServiceClient.GetTableClient(tableName);
            
            if(table == null) 
            {
                return BadRequest("Invalid Table name");
            }

            var result = await table.GetEntityIfExistsAsync<Employee>("IT", "1");

            if(result == null) 
            {
                return BadRequest("Invalid Data");
            }

            return(Ok(result.Value));

        }

        [HttpPost("publish")]
        public async Task<IActionResult> Publish([FromBody] Employee employee)
        {
            var tableName = "employee";

            await _tableServiceClient.CreateTableIfNotExistsAsync(tableName);

            var table = _tableServiceClient.GetTableClient("employee");

            await table.AddEntityAsync<Employee>(employee);

            return Created("", employee);
        }

        [HttpGet("query")]
        public IActionResult Query()
        {
            var tableName = "employee";

            var table = _tableServiceClient.GetTableClient(tableName);

            if (table == null)
            {
                return BadRequest("Invalid Table name");
            }

            var result = table.Query<Employee>(e => e.PartitionKey == "IT");
            

            return Ok(result);

        }

    }
}
