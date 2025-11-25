using CattleystOutboxWorker.Interfaces;

namespace CattleystOutboxWorker.Implementations
{
    internal class InboxEventPublisher : IEventPublisher
    {
        private readonly ILogger<InboxEventPublisher> _logger;

        public InboxEventPublisher(ILogger<InboxEventPublisher> logger)
        {
            _logger = logger;
        }

        public Task PublishAsync(string payload, byte eventTypeCode)
        {
            


        }
    }
}
