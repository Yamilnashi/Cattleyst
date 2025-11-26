using CattleystData.Implementations;
using CattleystData.Interfaces;
using CattleystWebPortal.Implementations;
using CattleystWebPortal.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CattleystWebPortal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            string? hostAddress = builder.Configuration["Api:CattleystApiHost"];
            if (string.IsNullOrEmpty(hostAddress))
            {
                throw new ArgumentNullException(nameof(hostAddress));
            }            
            builder.Services.AddHttpClient("CattleystWebApi", serviceProvider =>
            {
                serviceProvider.BaseAddress = new Uri(hostAddress);
            });

            builder.Services.AddSingleton<IApiService, ApiService>();

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
