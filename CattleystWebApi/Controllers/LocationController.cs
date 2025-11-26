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

        [HttpGet("list", Name = nameof(list))]
        public async Task<IActionResult> list()
        {
            IEnumerable<Location> locations = await _dbRead.LocationList();
            return Ok(locations);
        }

        [HttpGet("{locationId}", Name = nameof(Get))]
        public async Task<IActionResult> Get(int locationId)
        {
            Location location = await _dbRead.LocationGet(locationId);
            return Ok(location);
        }

        [HttpPost("Add", Name = nameof(Add))]
        public async Task<IActionResult> Add([FromBody] LocationAddRequest request)
        {
            await _dbWrite.LocationAdd(request.LocationName);
            return Ok();
        }

        [HttpPatch("{locationId}/Update", Name = nameof(Update))]
        public async Task<IActionResult> Update(int locationId, [FromBody] LocationAddRequest request)
        {
            await _dbWrite.LocationUpdate(locationId, request.LocationName);
            return Ok();
        }

        [HttpDelete("{locationId}/Delete", Name = nameof(Delete))]
        public async Task<IActionResult> Delete(int locationId)
        {
            await _dbWrite.LocationDelete(locationId);
            return Ok();
        }

    }
}
