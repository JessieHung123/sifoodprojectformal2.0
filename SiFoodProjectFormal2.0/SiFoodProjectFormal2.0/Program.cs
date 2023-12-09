using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.OData;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using SiFoodProjectFormal2._0.Models;


namespace SiFoodProjectFormal2._0
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddResponseCaching();
            // Add services to the container.
            builder.Services.AddDbContextPool<Sifood3Context>(options =>
            {
                options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("Sifood"));
            });

            builder.Services.AddControllersWithViews();
            builder.Services.AddControllers().AddOData(
                options => options.Select()
                                .Filter()
                                .Expand()
                                .SetMaxTop(100)
                                .Count()
                                .OrderBy()

                );
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddTransient<IUserIdentityService, UserIdentityService>();
            builder.Services.AddTransient<IStoreIdentityService, StoreIdentityService>();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Users/Account/LoginRegister";
                options.AccessDeniedPath = "/Account/LoginRegister";
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

            }
            app.UseResponseCaching();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "Users",
            //        pattern: "{area:exists}/{controller=Home}/{action=Main}/{id?}");

            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller=Home}/{action=Main}/{id?}");

            //});
            app.MapControllerRoute(
              name: "areas",
              pattern: "{area=Users}/{controller=Home}/{action=Main}/{id?}"
            );
            //app.MapControllerRoute(
            //    name: "default",
            //    pattern: "{area=Users}/{controller=Home}/{action=MainPage}/{id?}");
            //app.UseEndpoints(
            //    endpoints => { endpoints.MapAreaControllerRoute(name: "Users", areaName: "Users", pattern: "{action=MainPage}");
            //    endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Main}/{id?}"); });

            app.Run();
        }
    }
}