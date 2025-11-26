using CattleystData.Implementations;
using CattleystData.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CattleystWebPortal
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

            // Add services to the container.
            builder.Services.AddScoped<IDboDbReadContext, DboDbContext>(serviceProvider => new DboDbContext(connectionString));
            builder.Services.AddScoped<IDboDbWriteContext, DboDbContext>(serviceProvider => new DboDbContext(connectionString));

            builder.Services.AddControllersWithViews();
            string? hostAddress = builder.Configuration["Api:CattleystApiHost"];
            if (string.IsNullOrEmpty(hostAddress))
            {
                throw new ArgumentNullException(nameof(hostAddress));
            }            
            builder.Services.AddHttpClient("CattleystWebApi", serviceProvider => serviceProvider.BaseAddress = new Uri(hostAddress));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
