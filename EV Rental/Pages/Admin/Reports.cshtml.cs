using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.Enums;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Admin
{
    public class ReportsModel : PageModel
    {
        private readonly IReviewService _reviewService;
        private readonly IReportService _reportService;

        public ReportsModel(IReviewService reviewService, IReportService reportService)
        {
            _reviewService = reviewService;
            _reportService = reportService;
        }

        // Overview Stats
        public int TotalRentals { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalVehicles { get; set; }
        public int AvailableVehicles { get; set; }
        public double RentalGrowth { get; set; }
        public double RevenueGrowth { get; set; }

        // Rental Stats by Status
        public int PendingRentals { get; set; }
        public int ActiveRentals { get; set; }
        public int CompletedRentals { get; set; }
        public int CancelledRentals { get; set; }

        // Reviews
        public ReviewStatisticsDto ReviewStatistics { get; set; } = new();
        public IEnumerable<RatingReviewDto> Reviews { get; set; } = new List<RatingReviewDto>();

        // Recent Rentals
        public List<BusinessLayer.Services.RentalSummaryDto> RecentRentals { get; set; } = new();

        // Charts Data
        public List<decimal> MonthlyRevenue { get; set; } = new();
        public List<string> TopVehicleNames { get; set; } = new();
        public List<decimal> TopVehicleRevenue { get; set; } = new();

        public async Task OnGetAsync()
        {
            await LoadOverviewStatsAsync();
            await LoadRentalStatsAsync();
            await LoadReviewsAsync();
            await LoadRecentRentalsAsync();
            await LoadChartsDataAsync();
        }

        private async Task LoadOverviewStatsAsync()
        {
            var stats = await _reportService.GetOverviewStatsAsync();
            
            TotalRentals = stats.TotalRentals;
            TotalRevenue = stats.TotalRevenue;
            TotalVehicles = stats.TotalVehicles;
            AvailableVehicles = stats.AvailableVehicles;
            RentalGrowth = stats.RentalGrowth;
            RevenueGrowth = stats.RevenueGrowth;
        }

        private async Task LoadRentalStatsAsync()
        {
            var stats = await _reportService.GetRentalStatsAsync();
            
            PendingRentals = stats.PendingRentals;
            ActiveRentals = stats.ActiveRentals;
            CompletedRentals = stats.CompletedRentals;
            CancelledRentals = stats.CancelledRentals;
        }

        private async Task LoadReviewsAsync()
        {
            // Get statistics
            var statsResult = await _reviewService.GetReviewStatisticsAsync();
            if (statsResult.Success && statsResult.Data != null)
            {
                ReviewStatistics = statsResult.Data;
            }

            // Get recent reviews
            var reviewsResult = await _reviewService.GetAllReviewsAsync();
            if (reviewsResult.Success && reviewsResult.Data != null)
            {
                Reviews = reviewsResult.Data.OrderByDescending(r => r.CreatedAt);
            }
        }

        private async Task LoadRecentRentalsAsync()
        {
            RecentRentals = await _reportService.GetRecentRentalsAsync(10);
        }

        private async Task LoadChartsDataAsync()
        {
            var currentYear = DateTime.Now.Year;
            
            // Monthly revenue for current year
            MonthlyRevenue = await _reportService.GetMonthlyRevenueAsync(currentYear);

            // Top 5 vehicles by revenue
            var topVehicles = await _reportService.GetTopVehiclesByRevenueAsync(5);
            TopVehicleNames = topVehicles.VehicleNames;
            TopVehicleRevenue = topVehicles.VehicleRevenues;
        }
    }
}

