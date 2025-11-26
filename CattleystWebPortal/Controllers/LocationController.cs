using CattleystData.Interfaces;
using CattleystData.Models;
using CattleystWebPortal.Filters;
using CattleystWebPortal.ViewModels.Locations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CattleystWebPortal.Controllers
{
    [Route("Location")]
    public class LocationController : Controller
    {

        private readonly ILogger<LocationController> _logger;
        private readonly IDboDbReadContext _dbRead;

        public LocationController(ILogger<LocationController> logger,
            IDboDbReadContext dbRead)
        {
            _logger = logger;
            _dbRead = dbRead;
        }

        #region GET
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            LocationIndexViewModel model = new LocationIndexViewModel();
            return View(model);
        }

        [HttpGet]
        [AjaxOnly]
        [Route("Modal")]
        public IActionResult Modal(int? locationId)
        {
            LocationModalViewModel model = new LocationModalViewModel()
            {
                LocationId = locationId
            };
            return PartialView(model);
        }

        #endregion

        #region POST
        [HttpPost]
        [AjaxOnly]
        [Route("ModalSave")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModalSave(int? locationId, string locationName)
        {
            await Task.CompletedTask;
            return Content("OK");
        }
        #endregion

        #region JSON
        [HttpGet]
        [AjaxOnly]
        [Route("LocationListTableJson")]
        public async Task<IActionResult> LocationListTableJson()
        {
            IEnumerable<Location> locations = await _dbRead.LocationList();
            string json = JsonConvert.SerializeObject(new { data = locations });
            return Content(json);
        }
        #endregion
    }
}
