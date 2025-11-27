using System;

namespace CattleystData.Models.Idempotency
{
    public class IdempotencyDTO
    {
        public Guid RequestId { get; set; }
        public string PayloadHash { get; set; }
    }
}
