
using EmployeeManagament.Models;
using EmployeeManagament.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;

namespace EmployeeManagament
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configValue = builder.Configuration["AppSettings:SiteTitle"];

            //Services
            // appsetting 1-- configuraing poco class
            builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));


            builder.Services.AddDbContextPool<AppDbContext>(options =>
              options.UseSqlServer(builder.Configuration.GetConnectionString("EmployeeDBConnection")));


            //builder.Services.AddControllersWithViews(); // Replaces AddMvc()

            // Global authorization filter
            // This ensures that all controllers/actions require authenticated users by default — you don’t have to add [Authorize] manually to each one.You can still override this by using [AllowAnonymous] on specific controllers/actions.
            builder.Services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });
            builder.Services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();
            // appsetting 2
            var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();
            
            // without Password Configuration
            //builder.Services.AddIdentity<IdentityUser, IdentityRole>()

            // Password Configuration

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequiredLength = 5;
                    options.Password.RequiredUniqueChars = 2;
                    options.Password.RequireNonAlphanumeric = false;
                }).AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();


            var app = builder.Build();


            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }




           // Process currentProcess = Process.GetCurrentProcess();

            //Middleware

  
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication(); // 🔑 Must come before UseAuthorization
            app.UseAuthorization();



            // Default MVC route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Minimal API test endpoint
            // app.MapGet("/", () => "Hello, world!");
            //app.MapGet("/crash", () =>
            //{
            //    throw new Exception("Something went wrong in /crash endpoint");
            //});





            app.Run();
        }
    }
}