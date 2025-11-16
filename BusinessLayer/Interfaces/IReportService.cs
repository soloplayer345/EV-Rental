using BusinessLayer.Services;

namespace BusinessLayer.Interfaces
{
    public interface IReportService
    {
        Task<OverviewStatsDto> GetOverviewStatsAsync();
        Task<RentalStatsDto> GetRentalStatsAsync();
        Task<List<RentalSummaryDto>> GetRecentRentalsAsync(int count = 10);
        Task<List<decimal>> GetMonthlyRevenueAsync(int year);
        Task<TopVehiclesDto> GetTopVehiclesByRevenueAsync(int count = 5);
        Task<List<StationRevenueDto>> GetRevenueByStationAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<List<DailyFrequencyDto>> GetDailyRentalFrequencyAsync(DateTime startDate, DateTime endDate);
        Task<List<VehicleTypeStatsDto>> GetTopVehicleTypesByRentalCountAsync();
        Task<List<VehicleRentalStatsDto>> GetMostRentedVehiclesAsync(int count = 10);
        Task<List<InspectionProblemReportDto>> GetAllInspectionProblemsAsync(string? incidentType = null, int? rentalId = null);
        Task<int> GetNewReportsCountAsync();
    }
}
