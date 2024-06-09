using AzureSql.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzureSql.Controllers
{
    [ApiController]
    [Route("api")]
    public class AzureSqlController:ControllerBase
    {
        private readonly AzureCourseDbContext _dbContext;

        public AzureSqlController(AzureCourseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers() 
        {
            var result = await _dbContext
                .Customers
                .Include(pr=>pr.CustomerAddresses)
                .Take(50)
                .ToListAsync();

            return Ok(result);
        }
    }
}
