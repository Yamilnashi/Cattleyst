namespace CattleystData.Models.Enums
{
    public enum ERequestState : byte
    {
        Pending = 1,
        Processing = 2,
        Completed = 3,
        Failed = 4,
        TimedOUt = 5,
        Cancelled = 6,
        Validated = 7,
        Retried = 8,
        Aborted = 9,
        Archived = 10
    }
}
