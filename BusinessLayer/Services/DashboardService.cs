using BusinessLayer.DTOs;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Services
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardAsync();
    }

    public class DashboardService : IDashboardService
    {
        private readonly UnitOfWork _unitOfWork;
        public DashboardService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardDto> GetDashboardAsync()
        {
            var totalAccounts = await _unitOfWork.AccountRepo.CountAsync();
            var totalVehicles = await _unitOfWork.VehicleRepo.CountAsync();
            var totalRentals = await _unitOfWork.RentalRecordRepo.CountAsync();
            var totalRevenue = await _unitOfWork.PaymentRepo.SumAsync(p => p.Amount);

            return new DashboardDto
            {
                TotalAccounts = totalAccounts,
                TotalVehicles = totalVehicles,
                TotalRentals = totalRentals,
                TotalRevenue = totalRevenue
            };
        }
    }
}