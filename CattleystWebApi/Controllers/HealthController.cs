using CattleystData.Implementations;
using CattleystData.Models;
using Microsoft.AspNetCore.Mvc;

namespace CattleystWebApi.Controllers
{
    [ApiController]
    [Route("Health")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;
        private readonly IConfiguration _config;

        public HealthController(ILogger<HealthController> logger,
            IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            HealthCheckResult result = new HealthCheckResult();
            try
            {
                string? connectionString = _config.GetConnectionString("dbCattleyst");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new ArgumentNullException(nameof(connectionString));
                }

                using (DboDbContext connection = new DboDbContext(connectionString))
                {
                    result.DbConnection = "Healthy";
                }
            } catch (Exception ex)
            {
                _logger.LogError("Error checking health of DB: {ex}.", ex);
                result.DbConnection = "Unhealthy";
            }
            return Ok(result);
        }
    }
}
