using CattleystData.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace CattleystWebApi.DTO
{
    public record CattleAddRequest
    {
        public int LocationId { get; set; }
        public ECattleType CattleTypeCode { get; set; }
        public DateTime Birthdate { get; set; }
    }
}
