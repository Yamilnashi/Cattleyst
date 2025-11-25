namespace CattleystOutboxWorker.Interfaces
{
    internal interface IEventPublisher
    {
        Task PublishAsync(string payload, byte eventTypeCode);
    }
}
