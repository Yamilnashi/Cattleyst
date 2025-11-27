using CattleystData.Models;
using CattleystWebPortal.Filters;
using CattleystWebPortal.Interfaces;
using CattleystWebPortal.Models;
using CattleystWebPortal.Models.Apis;
using CattleystWebPortal.ViewModels.Locations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CattleystWebPortal.Controllers
{
    [Route("Location")]
    public class LocationController : Controller
    {
        private readonly ILogger<LocationController> _logger;
        private readonly IApiService _apiService;

        public LocationController(ILogger<LocationController> logger,
            IApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;
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
        public async Task<IActionResult> Modal(int? locationId)
        {
            LocationModalViewModel model = new LocationModalViewModel()
            {
                LocationId = locationId
            };

            if (locationId != null)
            {
                ApiResult<Location?> result = await _apiService.GetAsync<Location?>($"location/{locationId}");
                if (result.IsSuccess && 
                    result.Data != null)
                {
                    model.LocationName = result.Data.LocationName;
                }
            }

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
            object data = new { locationName = locationName.Trim() };
            if (locationId == null)
            {
                // new location                              
                await _apiService.PostAsJsonAsync("location/add", data);
            } else
            {
                // update location
               await _apiService.PatchAsJsonAsync($"location/{locationId}/update", data);
            }
            return Content("OK");
        }

        [HttpPost]
        [AjaxOnly]
        [Route("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int locationId)
        {
            await _apiService.DeleteAsync($"location/{locationId}/delete");
            return Content("OK");
        }

        #endregion

        #region JSON
        [HttpGet]
        [AjaxOnly]
        [Route("LocationListTableJson")]
        public async Task<IActionResult> LocationListTableJson()
        {
            IEnumerable<Location> locations = [];
            ApiResult<IEnumerable<Location>> result = await _apiService.GetAsync<IEnumerable<Location>>("location/list");
            if (result.IsSuccess && 
                result.Data != null)
            {
                locations = result.Data;
            }
            string json = JsonConvert.SerializeObject(new { data = locations });
            return Content(json);
        }
        #endregion
    }
}
