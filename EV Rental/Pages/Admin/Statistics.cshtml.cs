using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Admin
{
    public class StatisticsModel : PageModel
    {
        private readonly ReportService _reportService;

        public StatisticsModel(ReportService reportService)
        {
            _reportService = reportService;
        }

        public List<StationRevenueDto> StationRevenue { get; set; } = new();
        public List<DailyFrequencyDto> DailyFrequency { get; set; } = new();
        public List<VehicleTypeStatsDto> VehicleTypeStats { get; set; } = new();
        public List<VehicleRentalStatsDto> MostRentedVehicles { get; set; } = new();

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public async Task<IActionResult> OnGetAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            // Check admin authorization
            if (!EV_Rental.Helpers.SessionHelper.IsAdmin(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            // Default to last 30 days if not specified
            StartDate = startDate ?? DateTime.Now.AddDays(-30);
            EndDate = endDate ?? DateTime.Now;

            // Load statistics
            StationRevenue = await _reportService.GetRevenueByStationAsync(StartDate, EndDate);
            DailyFrequency = await _reportService.GetDailyRentalFrequencyAsync(StartDate.Value, EndDate.Value);
            VehicleTypeStats = await _reportService.GetTopVehicleTypesByRentalCountAsync();
            MostRentedVehicles = await _reportService.GetMostRentedVehiclesAsync(10);

            return Page();
        }
    }
}

