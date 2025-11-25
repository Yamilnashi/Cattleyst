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
            var builder = Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<OutboxMessagesWorker>();
                    string? connectionString = hostContext.Configuration.GetConnectionString("dbCattleyst");
                    if (string.IsNullOrEmpty(connectionString))
                    {
                        throw new Exception("ConnectionString cannot be null.");
                    }

                    services.AddTransient<IIdpyDbReadContext, IdpyDbContext>(serviceProvider => new IdpyDbContext(connectionString));
                    services.AddTransient<IIdpyDbWriteContext, IdpyDbContext>(serviceProvider => new IdpyDbContext(connectionString));
                    services.AddSingleton<IOutboxService, OutboxService>();
                });

            var host = builder.Build();
            host.Run();
        }
    }
}