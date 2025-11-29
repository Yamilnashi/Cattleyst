using CattleystData.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CattleystWebPortal.ViewModels.Cattles
{
    public class CattleIndexViewModel
    {
        public int? LocationId { get; set; }
        public List<SelectListItem> LocationListItems { get; set; } = [];

        public string ColDefs_CattleTableViewModel
        {
            get
            {
                return new Mapper().GetColumnsJson(typeof(CattleTableViewModel));
            }
        }

    }
}
