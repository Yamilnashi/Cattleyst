using CattleystData.Data;
using CattleystData.Models.Enums;

namespace CattleystWebPortal.ViewModels.Cattles
{
    public record CattleTableViewModel
    {
        [DataTableColumn(0, "", orderable: false)]
        public int CattleId { get; set; }
        [DataTableColumn(1, "Location")]
        public int LocationId { get; set; }
        [DataTableColumn(2, "Type")]
        public ECattleType CattleTypeCode { get; set; }
        [DataTableColumn(3, "DOB")]
        public DateTime Birthdate { get; set; }
        [DataTableColumn(4, "Last Updated")]
        public DateTime SavedDate { get; set; }
    }
}
