using CattleystData.Interfaces;
using CattleystData.Models;
using Microsoft.AspNetCore.Mvc;

namespace CattleystWebApi.Controllers
{
    [ApiController]
    [Route("Cattle")]
    public class CattleController : ControllerBase
    {
        private readonly ILogger<CattleController> _logger;
        private readonly IDboDbReadContext _dbRead;

        public CattleController(ILogger<CattleController> logger, 
            IDboDbReadContext dbRead)
        {
            _logger = logger;
            _dbRead = dbRead;
        }

        [HttpGet("list", Name = nameof(CattleList))]
        public async Task<IActionResult> CattleList([FromQuery] int[]? locationId = null)
        {
            IEnumerable<Cattle> cattle = await _dbRead.CattleList(locationId);
            return Ok(cattle);
        }

    }
}
