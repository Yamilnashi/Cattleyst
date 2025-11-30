using CattleystData.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CattleystWebPortal.ViewModels.Cattles
{
    public class CattleModalViewModel
    {
        public int? CattleId { get; set; }
        public int? LocationId { get; set; }
        public ECattleType? CattleTypeCode { get; set; }
        public DateTime? Birthdate { get; set; }
        public List<SelectListItem> LocationListItems { get; set; } = [];
        public List<SelectListItem> CattleTypeListItems
        {
            get
            {
                return Enum.GetValues(typeof(ECattleType))
                    .Cast<ECattleType>()
                    .Select(x => new SelectListItem()
                    {
                        Text = x.ToString(),
                        Value = ((byte)x).ToString()
                    })
                    .ToList();
            }
        }
    }
}
