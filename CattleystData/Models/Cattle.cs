using CattleystData.Models.Enums;
using System;

namespace CattleystData.Models
{
    public class Cattle
    {
        public int CattleId { get; set; }
        public int LocationId { get; set; }
        public ECattleType CattleTypeCode { get; set; }
        public DateTime Birthdate { get; set; }
        public DateTime SavedDate { get; set; }
    }
}
