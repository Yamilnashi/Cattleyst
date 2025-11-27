using CattleystData.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CattleystWebPortal.ViewModels.Cattles
{
    public class CattleIndexViewModel
    {
        public int? LocationId { get; set; }
        public List<SelectListItem> LocationListItems { get; set; } = [];

    }
}
