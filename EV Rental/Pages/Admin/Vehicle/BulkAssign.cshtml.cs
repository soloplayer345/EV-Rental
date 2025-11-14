using BusinessLayer.DTOs;
using BusinessLayer.Services;
using DataAccessLayer.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Admin.Vehicle
{
    public class BulkAssignModel : PageModel
    {
        private readonly VehicleService _vehicleService;
        private readonly StationService _stationService;

        public BulkAssignModel(VehicleService vehicleService, StationService stationService)
        {
            _vehicleService = vehicleService;
            _stationService = stationService;
        }

        public List<VehicleDto> Vehicles { get; set; } = new();
        public List<StationDto> Stations { get; set; } = new();
        public int? CurrentStationFilter { get; set; }
        public string? StatusFilter { get; set; }

        public async Task<IActionResult> OnGetAsync(int? currentStation = null, string? status = null)
        {
            // Check admin authorization
            if (!EV_Rental.Helpers.SessionHelper.IsAdmin(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            CurrentStationFilter = currentStation;
            StatusFilter = status;

            // Load all stations
            Stations = (await _stationService.GetAllStationsAsync()).ToList();

            // Load vehicles with filters
            var allVehicles = await _vehicleService.GetVehiclesAsync();

            // Apply filters
            if (currentStation.HasValue)
            {
                allVehicles = allVehicles.Where(v => v.StationId == currentStation.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                var vehicleStatus = Enum.Parse<VehicleStatus>(status);
                allVehicles = allVehicles.Where(v => v.Status == vehicleStatus);
            }

            // Exclude rented vehicles
            Vehicles = allVehicles.Where(v => v.Status != VehicleStatus.Rented).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAssignAsync(string vehicleIds, int targetStationId, string? newStatus = null)
        {
            try
            {
                if (string.IsNullOrEmpty(vehicleIds))
                {
                    TempData["ErrorMessage"] = "Vui lòng chọn ít nhất một xe";
                    return RedirectToPage();
                }

                var ids = vehicleIds.Split(',').Select(int.Parse).ToList();

                // Update each vehicle
                foreach (var id in ids)
                {
                    var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
                    if (vehicle != null)
                    {
                        vehicle.StationId = targetStationId;

                        // Update status if specified
                        if (!string.IsNullOrEmpty(newStatus))
                        {
                            vehicle.Status = Enum.Parse<VehicleStatus>(newStatus);
                        }

                        await _vehicleService.UpdateVehicleAsync(vehicle);
                    }
                }

                var station = await _stationService.GetStationByIdAsync(targetStationId);
                TempData["SuccessMessage"] = $"Đã phân phối {ids.Count} xe đến trạm {station?.Name}";
                return RedirectToPage("/Admin/Vehicle/Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToPage();
            }
        }
    }
}

