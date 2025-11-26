using CattleystData.Data;

namespace CattleystWebPortal.ViewModels.Locations
{
    public class LocationIndexViewModel
    {
        public string ColDefs_Locations => new Mapper().GetColumnsJson(typeof(LocationListTableViewModel));
    }
}
