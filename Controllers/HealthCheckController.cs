using FFhub_backend.Database;
using FFhub_backend.HttpReponse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FFhub_backend.Controllers
{
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        private readonly FFDbContext _dbContext;
        public HealthCheckController(FFDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        [Route("api/health")]
        public async Task<IActionResult> Health()
        {
            var maybe = new Maybe<string>();
            try
            {
                // test data base connection is working
                var tag = await _dbContext.Tags.FirstOrDefaultAsync(e => e.TagId == 1);
                if (tag != null)
                {
                    maybe.SetSuccess("Healthy");
                }
                else
                {
                    maybe.SetException("tag is null");
                }
            }
            catch (Exception e)
            {
                maybe.SetException("Database error occured " + e.Message);
            }

            return Ok(maybe);
        }
    }
}
