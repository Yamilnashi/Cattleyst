using CattleystData.Interfaces;
using CattleystData.Models;
using CattleystWebApi.DTO;
using CattleystWebApi.Interfaces;
using CattleystWebApi.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace CattleystWebApi.Controllers
{
    [ApiController]
    [Route("Cattle")]
    public class CattleController : ControllerBase
    {
        private readonly ILogger<CattleController> _logger;
        private readonly IDboDbReadContext _dbRead;
        private readonly IDboDbWriteContext _dbWrite;
        private readonly ICacheService _cache;

        public CattleController(ILogger<CattleController> logger, 
            IDboDbReadContext dbRead,
            ICacheService cache,
            IDboDbWriteContext dbWrite)
        {
            _logger = logger;
            _dbRead = dbRead;
            _cache = cache;
            _dbWrite = dbWrite;
        }

        [HttpGet("list", Name = nameof(CattleList))]
        public async Task<IActionResult> CattleList([FromQuery] int[]? locationId = null)
        {            
            IEnumerable<Cattle>? cattle = await _cache.GetOrSetAsync(CacheKeyBuilder.CattleByLocations(locationId), async () => await _dbRead.CattleList(locationId));
            return Ok(cattle);
        }

        [HttpPost("add", Name = nameof(CattleAdd))]
        public async Task<IActionResult> CattleAdd([FromBody] CattleAddRequest request)
        {
            await _dbWrite.CattleAdd(request.LocationId, (byte)request.CattleTypeCode, request.Birthdate);
            await _cache.RemoveAsync(CacheKeyBuilder.CattleByLocations([request.LocationId]));
            await _cache.RemoveAsync(CacheKeyBuilder.CattleAll());
            return Ok();
        }
    }
}
