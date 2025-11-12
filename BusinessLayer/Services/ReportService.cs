using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using DataAccessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Services
{
    public class ReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Overview Stats
        public async Task<OverviewStatsDto> GetOverviewStatsAsync()
        {
            var rentalRepo = _unitOfWork.GetRepository<RentalRecord>();
            var vehicleRepo = _unitOfWork.GetRepository<Vehicle>();

            var allRentals = await rentalRepo.GetAllAsync();
            var allVehicles = await vehicleRepo.GetAllAsync();

            // Total rentals
            var totalRentals = allRentals.Count();

            // Total revenue
            var totalRevenue = allRentals
                .Where(r => r.Status == RentalRecordStatus.Completed)
                .Sum(r => r.TotalPrice);

            // Total vehicles
            var totalVehicles = allVehicles.Count();

            // Available vehicles
            var availableVehicles = allVehicles.Count(v => v.Status == VehicleStatus.Available);

            // Calculate growth (last 30 days vs previous 30 days)
            var last30Days = DateTime.Now.AddDays(-30);
            var previous60Days = DateTime.Now.AddDays(-60);

            var recentCount = allRentals.Count(r => r.CreateDate >= last30Days);
            var previousCount = allRentals.Count(r => r.CreateDate >= previous60Days && r.CreateDate < last30Days);

            var rentalGrowth = previousCount > 0
                ? Math.Round(((recentCount - previousCount) / (double)previousCount) * 100, 1)
                : 0;

            var recentRevenue = allRentals
                .Where(r => r.CreateDate >= last30Days && r.Status == RentalRecordStatus.Completed)
                .Sum(r => r.TotalPrice);

            var previousRevenue = allRentals
                .Where(r => r.CreateDate >= previous60Days && r.CreateDate < last30Days && r.Status == RentalRecordStatus.Completed)
                .Sum(r => r.TotalPrice);

            var revenueGrowth = previousRevenue > 0
                ? Math.Round(((double)(recentRevenue - previousRevenue) / (double)previousRevenue) * 100, 1)
                : 0;

            return new OverviewStatsDto
            {
                TotalRentals = totalRentals,
                TotalRevenue = totalRevenue,
                TotalVehicles = totalVehicles,
                AvailableVehicles = availableVehicles,
                RentalGrowth = rentalGrowth,
                RevenueGrowth = revenueGrowth
            };
        }

        // Rental Stats by Status
        public async Task<RentalStatsDto> GetRentalStatsAsync()
        {
            var rentalRepo = _unitOfWork.GetRepository<RentalRecord>();
            var allRentals = await rentalRepo.GetAllAsync();

            return new RentalStatsDto
            {
                PendingRentals = allRentals.Count(r => r.Status == RentalRecordStatus.Pending),
                ActiveRentals = allRentals.Count(r => r.Status == RentalRecordStatus.Active),
                CompletedRentals = allRentals.Count(r => r.Status == RentalRecordStatus.Completed),
                CancelledRentals = allRentals.Count(r => r.Status == RentalRecordStatus.Cancelled)
            };
        }

        // Recent Rentals
        public async Task<List<RentalSummaryDto>> GetRecentRentalsAsync(int count = 10)
        {
            var rentalRepo = _unitOfWork.GetRepository<RentalRecord>();
            var rentals = rentalRepo.GetAllQueryable("Renter,Vehicle");
            
            var recentRentals = await rentals
                .OrderByDescending(r => r.CreateDate)
                .Take(count)
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

            return recentRentals;
        }

        // Monthly Revenue Chart Data
        public async Task<List<decimal>> GetMonthlyRevenueAsync(int year)
        {
            var rentalRepo = _unitOfWork.GetRepository<RentalRecord>();
            var allRentals = await rentalRepo.GetAllAsync();

            var monthlyRevenue = new List<decimal>();

            for (int month = 1; month <= 12; month++)
            {
                var monthRevenue = allRentals
                    .Where(r => r.CreateDate.Year == year
                           && r.CreateDate.Month == month
                           && r.Status == RentalRecordStatus.Completed)
                    .Sum(r => r.TotalPrice);

                // Convert to millions for better chart display
                monthlyRevenue.Add(Math.Round(monthRevenue / 1000000, 2));
            }

            return monthlyRevenue;
        }

        // Top Vehicles by Revenue
        public async Task<TopVehiclesDto> GetTopVehiclesByRevenueAsync(int count = 5)
        {
            var rentalRepo = _unitOfWork.GetRepository<RentalRecord>();
            var rentals = rentalRepo.GetAllQueryable("Vehicle");

            var completedRentals = await rentals
                .Where(r => r.Status == RentalRecordStatus.Completed)
                .ToListAsync();

            var topVehicles = completedRentals
                .GroupBy(r => new { r.VehicleId, r.Vehicle.Name })
                .Select(g => new
                {
                    VehicleName = g.Key.Name ?? "Unknown",
                    Revenue = g.Sum(r => r.TotalPrice)
                })
                .OrderByDescending(x => x.Revenue)
                .Take(count)
                .ToList();

            return new TopVehiclesDto
            {
                VehicleNames = topVehicles.Select(v => v.VehicleName).ToList(),
                VehicleRevenues = topVehicles.Select(v => Math.Round(v.Revenue / 1000000, 2)).ToList()
            };
        }
    }

    // DTOs
    public class OverviewStatsDto
    {
        public int TotalRentals { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalVehicles { get; set; }
        public int AvailableVehicles { get; set; }
        public double RentalGrowth { get; set; }
        public double RevenueGrowth { get; set; }
    }

    public class RentalStatsDto
    {
        public int PendingRentals { get; set; }
        public int ActiveRentals { get; set; }
        public int CompletedRentals { get; set; }
        public int CancelledRentals { get; set; }
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

    public class TopVehiclesDto
    {
        public List<string> VehicleNames { get; set; } = new();
        public List<decimal> VehicleRevenues { get; set; } = new();
    }
}
