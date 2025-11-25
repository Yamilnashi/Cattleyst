using CattleystData.Models.Enums;
using System;

namespace CattleystData.Models.Idempotency
{
    public class OutboxMessage
    {
        public Guid OutboxMessageId { get; set; }
        public EEventType EventTypeCode { get; set; }
        public string Payload { get; set; }
        public DateTime SavedDate { get; set; }
        public DateTime ProcessedDate { get; set; }
    }
}
