using CattleystData.Models.Enums;
using System;

namespace CattleystData.Models.Idempotency
{
    public class IdempotencyRequest
    {
        public Guid RequestId { get; set; }
        public ERequestState RequestStateCode { get; set; }
        public string RequestHash { get; set; }
        public string ResponseJson { get; set; }
        public int? StatusCode { get; set; }
        public DateTime SavedDate { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
