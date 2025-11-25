using CattleystData.Implementations;
using CattleystData.Interfaces;
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

            string? connectionString = builder.Configuration.GetConnectionString("dbCattleyst");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("ConnectionString cannot be null.");
            }

            builder.Services.AddScoped<DbContext>(serviceProvider => new DbContext(connectionString));
            builder.Services.AddScoped<IDbReadContext, DbContext>();
            builder.Services.AddScoped<IDbWriteContext, DbContext>();
            builder.Services.AddSingleton<IEventPublisher, InboxEventPublisher>();

            var host = builder.Build();
            host.Run();
        }
    }
}