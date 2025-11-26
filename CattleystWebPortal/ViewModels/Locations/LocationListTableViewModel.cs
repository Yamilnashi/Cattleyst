using CattleystData.Data;

namespace CattleystWebPortal.ViewModels.Locations
{
    public class LocationListTableViewModel
    {
        [DataTableColumn(0, "", orderable: false)]
        public int LocationId { get; set; }
        [DataTableColumn(1, "Location")]
        public string LocationName { get; set; } = string.Empty;
    }
}
