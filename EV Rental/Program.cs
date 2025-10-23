using BusinessLayer.Interfaces;
using BusinessLayer.Services;
using DataAccessLayer;
using DataAccessLayer.Interfaces;
using EV_Rental.Middlewares;
using Microsoft.EntityFrameworkCore;

namespace EV_Rental
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            // Add Session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            //add connection String
            builder.Services.AddDbContext<EVRentalDBContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("LocalSQLServer"))
            );

            // Register UnitOfWork and Services
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession(); // Add Session middleware
            app.UseRoleBasedRedirect(); // Add Role-based redirect middleware
            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
