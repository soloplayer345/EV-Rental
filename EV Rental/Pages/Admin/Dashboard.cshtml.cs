using System;
using System.Collections.Generic;
using System.Linq;
using EV_Rental.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLayer.Interfaces;
using BusinessLayer.Services;
using DataAccessLayer.Enums;

namespace EV_Rental.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly IReportService _reportService;
        private readonly IVehicleService _vehicleService;
        
        public string UserEmail { get; set; } = string.Empty;
        public int NewReportsCount { get; set; } = 0;
        public OverviewStatsDto OverviewStats { get; set; } = new();
        public RentalStatsDto RentalStats { get; set; } = new();
        public List<RentalSummaryDto> RecentRentals { get; set; } = new();
        public List<decimal> MonthlyRevenue { get; set; } = new();
        public List<VehicleRentalStatsDto> TopVehicles { get; set; } = new();
        public Dictionary<VehicleStatus, int> VehicleStatusDistribution { get; set; } = new();
        public int SelectedYear { get; set; } = DateTime.Now.Year;

        public DashboardModel(IReportService reportService, IVehicleService vehicleService)
        {
            _reportService = reportService;
            _vehicleService = vehicleService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Kiểm tra quyền Admin
            if (!SessionHelper.IsAdmin(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            var user = SessionHelper.GetUserSession(HttpContext.Session);
            UserEmail = user?.Email ?? "";

            // lấy data
            OverviewStats = await _reportService.GetOverviewStatsAsync();
            RentalStats = await _reportService.GetRentalStatsAsync();
            RecentRentals = (await _reportService.GetRecentRentalsAsync(6)) ?? new List<RentalSummaryDto>();
            MonthlyRevenue = (await _reportService.GetMonthlyRevenueAsync(SelectedYear)) ?? new List<decimal>();
            TopVehicles = (await _reportService.GetMostRentedVehiclesAsync(4)) ?? new List<VehicleRentalStatsDto>();
            NewReportsCount = await _reportService.GetNewReportsCountAsync();

            
            var vehicles = (await _vehicleService.GetVehiclesAsync())?.ToList() ?? new List<BusinessLayer.DTOs.VehicleDto>();
            var statusDict = Enum.GetValues(typeof(VehicleStatus))
                .Cast<VehicleStatus>()
                .ToDictionary(status => status, status => 0);

            foreach (var group in vehicles.GroupBy(v => v.Status))
            {
                statusDict[group.Key] = group.Count();
            }

            VehicleStatusDistribution = statusDict;

            return Page();
        }
    }
}

