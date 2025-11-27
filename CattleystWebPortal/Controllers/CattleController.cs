using CattleystData.Models;
using CattleystData.Models.Enums;
using CattleystWebPortal.Interfaces;
using CattleystWebPortal.Models.Apis;
using CattleystWebPortal.ViewModels.Cattles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

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
    }
}
