using CattleystData.Models;
using CattleystWebPortal.Filters;
using CattleystWebPortal.ViewModels.Locations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CattleystWebPortal.Controllers
{
    [Route("Location")]
    public class LocationController : Controller
    {
        private readonly ILogger<LocationController> _logger;
        private readonly HttpClient _client;

        public LocationController(ILogger<LocationController> logger,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _client = httpClientFactory.CreateClient("CattleystWebApi");
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
                HttpResponseMessage? response = await _client.GetAsync($"location/{(int)locationId}");
                if (response.IsSuccessStatusCode)
                {
                    string? content = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(content))
                    {
                        Location? location = JsonConvert.DeserializeObject<Location>(content);
                        if (location != null)
                        {
                            model.LocationName = location.LocationName;
                        }
                    }
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
            HttpResponseMessage? response;
            var data = new { locationName = locationName.Trim() };
            if (locationId == null)
            {
                // new location
                              
                response = await _client.PostAsJsonAsync("location/add", data);
            } else
            {
                // update location
                response = await _client.PutAsJsonAsync($"location/{locationId}/update", data);
            }
            if (!response.IsSuccessStatusCode)
            {
                return Content("Failed");
            }
            return Content("OK");
        }

        [HttpPost]
        [AjaxOnly]
        [Route("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int locationId)
        {
            HttpResponseMessage? response;
            response = await _client.DeleteAsync($"location/{locationId}/delete");            
            if (!response.IsSuccessStatusCode)
            {
                return Content("Failed");
            }
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
            HttpResponseMessage? response = await _client.GetAsync("location/list");
            if (response.IsSuccessStatusCode)
            {
                string? content = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(content))
                {
                    IEnumerable<Location>? models = JsonConvert.DeserializeObject<IEnumerable<Location>>(content);
                    if (models != null)
                    {
                        locations = models;
                    }
                }                
            }
            string json = JsonConvert.SerializeObject(new { data = locations });
            return Content(json);
        }
        #endregion
    }
}
