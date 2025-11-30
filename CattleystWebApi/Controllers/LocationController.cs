using CattleystData.Interfaces;
using CattleystData.Models;
using CattleystWebApi.DTO;
using CattleystWebApi.Interfaces;
using CattleystWebApi.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CattleystWebApi.Controllers
{
    [ApiController]
    [Route("Location")]
    public class LocationController : ControllerBase
    {
        private readonly ILogger<LocationController> _logger;
        private readonly IDboDbReadContext _dbRead;
        private readonly IDboDbWriteContext _dbWrite;
        private readonly ICacheService _cache;

        public LocationController(ILogger<LocationController> logger,
            IDboDbReadContext dbRead,
            IDboDbWriteContext dbWrite,
            ICacheService cache)
        {
            _logger = logger;
            _dbRead = dbRead;
            _dbWrite = dbWrite;
            _cache = cache;
        }

        [HttpGet("list", Name = nameof(LocationList))]
        public async Task<IActionResult> LocationList()
        {
            IEnumerable<Location>? locations = await _cache.GetOrSetAsync(CacheKeyBuilder.AllLocations, () => _dbRead.LocationList());
            return Ok(locations);
        }

        [HttpGet("{locationId}", Name = nameof(LocationGet))]
        public async Task<IActionResult> LocationGet(int locationId)
        {
            string cacheKey = CacheKeyBuilder.LocationById(locationId);
            Location? location = await _cache.GetOrSetAsync(cacheKey, () => _dbRead.LocationGet(locationId));
            return Ok(location);
        }

        [HttpPost("Add", Name = nameof(LocationAdd))]
        public async Task<IActionResult> LocationAdd([FromBody] LocationAddRequest request)
        {
            await _dbWrite.LocationAdd(request.LocationName);
            await _cache.RemoveAsync(CacheKeyBuilder.AllLocations);
            return Ok();
        }

        [HttpPatch("{locationId}/Update", Name = nameof(LocationUpdate))]
        public async Task<IActionResult> LocationUpdate(int locationId, [FromBody] LocationAddRequest request)
        {
            await _dbWrite.LocationUpdate(locationId, request.LocationName);
            await _cache.RemoveAsync(CacheKeyBuilder.LocationById(locationId));
            await _cache.RemoveAsync(CacheKeyBuilder.AllLocations);
            return Ok();
        }

        [HttpDelete("{locationId}/Delete", Name = nameof(LocationDelete))]
        public async Task<IActionResult> LocationDelete(int locationId)
        {
            await _dbWrite.LocationDelete(locationId);
            await _cache.RemoveAsync(CacheKeyBuilder.LocationById(locationId));
            await _cache.RemoveAsync(CacheKeyBuilder.AllLocations);
            return Ok();
        }

    }
}
