using CattleystOutboxWorker.Implementations;
using CattleystOutboxWorker.Interfaces;

namespace CattleystOutboxWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<OutboxMessagesWorker>();
            builder.Services.AddSingleton<IEventPublisher, InboxEventPublisher>();
            var host = builder.Build();
            host.Run();
        }
    }
}