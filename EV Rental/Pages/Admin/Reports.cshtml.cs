using BusinessLayer.DTOs;
using BusinessLayer.Services;
using DataAccessLayer;
using DataAccessLayer.Enums;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EV_Rental.Pages.Admin
{
    public class ReportsModel : PageModel
    {
        private readonly ReviewService _reviewService;
        private readonly EVRentalDBContext _context;

        public ReportsModel(ReviewService reviewService, EVRentalDBContext context)
        {
            _reviewService = reviewService;
            _context = context;
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
        public List<RentalSummaryDto> RecentRentals { get; set; } = new();

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
            // Total rentals
            TotalRentals = await _context.RentalRecords.CountAsync();

            // Total revenue
            TotalRevenue = await _context.RentalRecords
                .Where(r => r.Status == RentalRecordStatus.Completed)
                .SumAsync(r => r.TotalPrice);

            // Total vehicles
            TotalVehicles = await _context.Vehicles.CountAsync();

            // Available vehicles
            AvailableVehicles = await _context.Vehicles
                .CountAsync(v => v.Status == VehicleStatus.Available);

            // Calculate growth (example - last 30 days vs previous 30 days)
            var last30Days = DateTime.Now.AddDays(-30);
            var previous60Days = DateTime.Now.AddDays(-60);

            var recentCount = await _context.RentalRecords
                .CountAsync(r => r.CreateDate >= last30Days);
            var previousCount = await _context.RentalRecords
                .CountAsync(r => r.CreateDate >= previous60Days && r.CreateDate < last30Days);

            RentalGrowth = previousCount > 0 
                ? Math.Round(((recentCount - previousCount) / (double)previousCount) * 100, 1)
                : 0;

            var recentRevenue = await _context.RentalRecords
                .Where(r => r.CreateDate >= last30Days && r.Status == RentalRecordStatus.Completed)
                .SumAsync(r => r.TotalPrice);
            var previousRevenue = await _context.RentalRecords
                .Where(r => r.CreateDate >= previous60Days && r.CreateDate < last30Days && r.Status == RentalRecordStatus.Completed)
                .SumAsync(r => r.TotalPrice);

            RevenueGrowth = previousRevenue > 0
                ? Math.Round(((double)(recentRevenue - previousRevenue) / (double)previousRevenue) * 100, 1)
                : 0;
        }

        private async Task LoadRentalStatsAsync()
        {
            PendingRentals = await _context.RentalRecords
                .CountAsync(r => r.Status == RentalRecordStatus.Pending);

            ActiveRentals = await _context.RentalRecords
                .CountAsync(r => r.Status == RentalRecordStatus.Active);

            CompletedRentals = await _context.RentalRecords
                .CountAsync(r => r.Status == RentalRecordStatus.Completed);

            CancelledRentals = await _context.RentalRecords
                .CountAsync(r => r.Status == RentalRecordStatus.Cancelled);
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
            RecentRentals = await _context.RentalRecords
                .Include(r => r.Renter)
                .Include(r => r.Vehicle)
                .OrderByDescending(r => r.CreateDate)
                .Take(10)
                .Select(r => new RentalSummaryDto
                {
                    Id = r.Id,
                    RenterName = r.Renter.FullName,
                    VehicleName = r.Vehicle.Name ?? "Unknown",
                    StartTime = r.StartTime,
                    TotalPrice = r.TotalPrice,
                    Status = r.Status
                })
                .ToListAsync();
        }

        private async Task LoadChartsDataAsync()
        {
            // Monthly revenue for current year
            var currentYear = DateTime.Now.Year;
            MonthlyRevenue = new List<decimal>();

            for (int month = 1; month <= 12; month++)
            {
                var monthRevenue = await _context.RentalRecords
                    .Where(r => r.CreateDate.Year == currentYear 
                           && r.CreateDate.Month == month 
                           && r.Status == RentalRecordStatus.Completed)
                    .SumAsync(r => r.TotalPrice);

                // Convert to millions for better chart display
                MonthlyRevenue.Add(Math.Round(monthRevenue / 1000000, 2));
            }

            // Top 5 vehicles by revenue
            var topVehicles = await _context.RentalRecords
                .Where(r => r.Status == RentalRecordStatus.Completed)
                .GroupBy(r => new { r.VehicleId, r.Vehicle.Name })
                .Select(g => new
                {
                    VehicleName = g.Key.Name ?? "Unknown",
                    Revenue = g.Sum(r => r.TotalPrice)
                })
                .OrderByDescending(x => x.Revenue)
                .Take(5)
                .ToListAsync();

            TopVehicleNames = topVehicles.Select(v => v.VehicleName).ToList();
            TopVehicleRevenue = topVehicles.Select(v => Math.Round(v.Revenue / 1000000, 2)).ToList();
        }
    }

    public class RentalSummaryDto
    {
        public int Id { get; set; }
        public string RenterName { get; set; } = string.Empty;
        public string VehicleName { get; set; } = string.Empty;
        public DateTime? StartTime { get; set; }
        public decimal TotalPrice { get; set; }
        public RentalRecordStatus Status { get; set; }
    }
}
