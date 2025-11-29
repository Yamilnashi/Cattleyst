using CattleystData.Interfaces;
using CattleystData.Models;
using CattleystWebApi.DTO;
using Microsoft.AspNetCore.Mvc;

namespace CattleystWebApi.Controllers
{
    [ApiController]
    [Route("Location")]
    public class LocationController : ControllerBase
    {
        private readonly ILogger<LocationController> _logger;
        private readonly IDboDbReadContext _dbRead;
        private readonly IDboDbWriteContext _dbWrite;

        public LocationController(ILogger<LocationController> logger,
            IDboDbReadContext dbRead,
            IDboDbWriteContext dbWrite)
        {
            _logger = logger;
            _dbRead = dbRead;
            _dbWrite = dbWrite;
        }

        [HttpGet("list", Name = nameof(LocationList))]
        public async Task<IActionResult> LocationList()
        {
            IEnumerable<Location> locations = await _dbRead.LocationList();
            return Ok(locations);
        }

        [HttpGet("{locationId}", Name = nameof(LocationGet))]
        public async Task<IActionResult> LocationGet(int locationId)
        {
            Location location = await _dbRead.LocationGet(locationId);
            return Ok(location);
        }

        [HttpPost("Add", Name = nameof(LocationAdd))]
        public async Task<IActionResult> LocationAdd([FromBody] LocationAddRequest request)
        {
            await _dbWrite.LocationAdd(request.LocationName);
            return Ok();
        }

        [HttpPatch("{locationId}/Update", Name = nameof(LocationUpdate))]
        public async Task<IActionResult> LocationUpdate(int locationId, [FromBody] LocationAddRequest request)
        {
            await _dbWrite.LocationUpdate(locationId, request.LocationName);
            return Ok();
        }

        [HttpDelete("{locationId}/Delete", Name = nameof(LocationDelete))]
        public async Task<IActionResult> LocationDelete(int locationId)
        {
            await _dbWrite.LocationDelete(locationId);
            return Ok();
        }

    }
}
