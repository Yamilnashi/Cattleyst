using CattleystData.Models;
using CattleystWebApi.DTO;
using CattleystWebPortal.Filters;
using CattleystWebPortal.Interfaces;
using CattleystWebPortal.Models.Apis;
using CattleystWebPortal.ViewModels.Cattles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace CattleystWebPortal.Controllers
{
    [Route("Cattle")]
    public class CattleController : Controller
    {
        private readonly ILogger<CattleController> _logger;
        private readonly IApiService _apiService;

        public CattleController(ILogger<CattleController> logger,
            IApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            CattleIndexViewModel model = new CattleIndexViewModel();
            ApiResult<IEnumerable<Location>> result = await _apiService.GetAsync<IEnumerable<Location>>("location/list");
            if (result.IsSuccess &&
                result.Data != null)
            {
                model.LocationListItems = result.Data.Select(x => new SelectListItem()
                {
                    Text = x.LocationName,
                    Value = x.LocationId.ToString()
                }).ToList();
            }
            return View(model);
        }

        [HttpGet]
        [AjaxOnly]
        [Route("Modal")]
        public async Task<IActionResult> Modal(int? cattleId)
        {
            CattleModalViewModel model = new CattleModalViewModel()
            {
                CattleId = cattleId
            };

            ApiResult<IEnumerable<Location>> result = await _apiService.GetAsync<IEnumerable<Location>>("location/list");
            if (result.IsSuccess &&
                result.Data != null)
            {
                model.LocationListItems = result.Data.Select(x => new SelectListItem()
                {
                    Text = x.LocationName,
                    Value = x.LocationId.ToString()
                }).ToList();
            }

            return PartialView(model);
        }

        #region POST
        [HttpPost]
        [AjaxOnly]
        [ValidateAntiForgeryToken]
        [Route("ModalSave")]
        public async Task<IActionResult> ModalSave(CattleAddRequest dto)
        {
            await _apiService.PostAsJsonAsync("cattle/add", dto);
            return Content("OK");
        }
        #endregion

        #region JSON
        [HttpGet]
        [AjaxOnly]
        [Route("CattleListTableJson")]
        public async Task<IActionResult> CattleListTableJson(int[]? locationIds = null)
        {
            IEnumerable<Cattle> cattle = [];

            ApiResult<IEnumerable<Cattle>> result = locationIds == null
                ? await _apiService.GetAsync<IEnumerable<Cattle>>("cattle/list")
                : await _apiService.GetAsync<IEnumerable<Cattle>>("cattle/list", new Dictionary<string, object>
                {
                    { "locationId", locationIds }
                });

            if (result.IsSuccess &&
                result.Data != null)
            {
                cattle = result.Data;
            }

            string json = JsonConvert.SerializeObject(new { data = cattle });
            return Content(json, "application/json");
        }
        #endregion

    }
}
