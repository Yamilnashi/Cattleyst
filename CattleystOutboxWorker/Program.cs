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

            builder.Services.AddTransient<IIdpyDbReadContext, IdpyDbContext>(serviceProvider => new IdpyDbContext(connectionString));
            builder.Services.AddTransient<IIdpyDbWriteContext, IdpyDbContext>(serviceProvider => new IdpyDbContext(connectionString));
            builder.Services.AddSingleton<IOutboxService, OutboxService>();

            var host = builder.Build();
            host.Run();
        }
    }
}