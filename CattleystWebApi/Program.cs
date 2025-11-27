using CattleystWebApi.Extensions;
using CattleystData.Implementations;
using CattleystData.Interfaces;

namespace CattleystWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            string? connectionString = builder.Configuration.GetConnectionString("dbCattleyst");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));  
            }

            System.Transactions.TransactionManager.ImplicitDistributedTransactions = true;

            // Add services to the container.
            builder.Services.AddScoped<IIdpyDbReadContext, IdpyDbContext>(serviceProvider => new IdpyDbContext(connectionString));
            builder.Services.AddScoped<IIdpyDbWriteContext, IdpyDbContext>(serviceProvider => new IdpyDbContext(connectionString));
            builder.Services.AddScoped<IDboDbReadContext, DboDbContext>(serviceProvider => new DboDbContext(connectionString));
            builder.Services.AddScoped<IDboDbWriteContext, DboDbContext>(serviceProvider => new DboDbContext(connectionString));
            
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseIdempotency();
            app.MapControllers();
            app.Run();
        }
    }
}
