global using DataAccessLayer.Entities;
using BusinessLayer.Interfaces;
using BusinessLayer.Services;
using DataAccessLayer;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using EV_Rental.Helpers;
using EV_Rental.Hubs;
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

            // Add SignalR
            builder.Services.AddSignalR();

            // Add Session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Add connection String with Fallback mechanism
            var connectionString = DatabaseConnectionHelper.GetConnectionStringWithFallback(
                builder.Configuration
            );
            builder.Services.AddDbContext<EVRentalDBContext>(options =>
                options.UseSqlServer(
                    connectionString,
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(5),
                            errorNumbersToAdd: null
                        );
                        sqlOptions.CommandTimeout(30);
                    }
                )
            );

            // Register Repositories
            builder.Services.AddScoped<IVehicleRepo, VehicleRepo>();
            builder.Services.AddScoped<IAccountRepo, AccountRepo>();
            builder.Services.AddScoped<IRatingReviewRepo, RatingReviewRepo>();

            // Register UnitOfWork and Services
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IVehicleService, VehicleService>();
            builder.Services.AddScoped<IRentalService, RentalService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<ICheckInService, CheckInService>();
            builder.Services.AddScoped<IStationService, StationService>();
            builder.Services.AddScoped<IRentalRecordService, RentalRecordService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<IReportService, ReportService>();
            builder.Services.AddScoped<AccountService>();

            //AI register
            builder.Services.AddScoped<CarAiAssistantService>();


            // Register SignalR wrapper service
            builder.Services.AddScoped<EV_Rental.Services.VehicleHubService>();

            // Bind SMTP settings & register EmailSender
            builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
            builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();

            // Bind VNPay settings
            builder.Services.Configure<VNPaySettings>(builder.Configuration.GetSection("VNPay"));

            // Bind MoMo settings
            builder.Services.Configure<MoMoSettings>(builder.Configuration.GetSection("MoMo"));





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
            app.MapHub<VehicleHub>("/vehicleHub"); // Map SignalR Hub


            app.Run();
        }
    }
}
