using BusinessLayer.DTOs;
using BusinessLayer.Services;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using EV_Rental.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Admin.Vehicle
{
    public class EditModel : PageModel
    {
        private readonly VehicleService _vehicleService;
        private readonly StationService _stationService;

        [BindProperty]
        public DataAccessLayer.Entities.Vehicle Vehicle { get; set; } = new();

        public List<DataAccessLayer.Entities.Station> Stations { get; set; } = new();
        public string UserEmail { get; set; } = string.Empty;

        public EditModel(VehicleService vehicleService, StationService stationService)
        {
            _vehicleService = vehicleService;
            _stationService = stationService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Kiểm tra quyền Admin
            if (!SessionHelper.IsAdmin(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            var user = SessionHelper.GetUserSession(HttpContext.Session);
            UserEmail = user?.Email ?? "";

            try
            {
                Vehicle = await _vehicleService.GetVehicleByIdAsync(id);
                if (Vehicle == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy xe";
                    return RedirectToPage("/Admin/Vehicle/Index");
                }

                // Lấy danh sách trạm
                Stations = (await _stationService.GetAllStationsAsync()).ToList();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToPage("/Admin/Vehicle/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Kiểm tra quyền Admin
            if (!SessionHelper.IsAdmin(HttpContext.Session))
            {
                return Unauthorized();
            }

            try
            {
                // Log received vehicle data
                System.Diagnostics.Debug.WriteLine($"=== OnPostAsync Called ===");
                System.Diagnostics.Debug.WriteLine($"Vehicle ID: {Vehicle.Id}");
                System.Diagnostics.Debug.WriteLine($"Vehicle Name: {Vehicle.Name}");
                System.Diagnostics.Debug.WriteLine($"Vehicle Type: {Vehicle.VehicleType}");
                System.Diagnostics.Debug.WriteLine($"Vehicle Status: {Vehicle.Status}");
                System.Diagnostics.Debug.WriteLine($"Vehicle StationId: {Vehicle.StationId}");
                
                // Lấy danh sách trạm cho case lỗi
                Stations = (await _stationService.GetAllStationsAsync()).ToList();

                // If StationId is 0 (not selected in form), restore from database
                if (Vehicle.StationId <= 0)
                {
                    var existingVehicle = await _vehicleService.GetVehicleByIdAsync(Vehicle.Id);
                    if (existingVehicle != null)
                    {
                        Vehicle.StationId = existingVehicle.StationId;
                        System.Diagnostics.Debug.WriteLine($"StationId restored from DB: {Vehicle.StationId}");
                    }
                }

                // Validate data trước
                if (string.IsNullOrWhiteSpace(Vehicle.Name))
                {
                    ModelState.AddModelError("Vehicle.Name", "Tên xe không được trống");
                }

                if (Vehicle.StationId <= 0)
                {
                    ModelState.AddModelError("Vehicle.StationId", "Vui lòng chọn trạm");
                }

                if (string.IsNullOrWhiteSpace(Vehicle.Brand))
                {
                    ModelState.AddModelError("Vehicle.Brand", "Hãng không được trống");
                }

                if (string.IsNullOrWhiteSpace(Vehicle.PlateNumber))
                {
                    ModelState.AddModelError("Vehicle.PlateNumber", "Biển số không được trống");
                }

                if (string.IsNullOrWhiteSpace(Vehicle.Model))
                {
                    ModelState.AddModelError("Vehicle.Model", "Mẫu xe không được trống");
                }

                if (string.IsNullOrWhiteSpace(Vehicle.VehicleType))
                {
                    ModelState.AddModelError("Vehicle.VehicleType", "Loại xe không được trống");
                }

                if (Vehicle.PricePerHour <= 0)
                {
                    ModelState.AddModelError("Vehicle.PricePerHour", "Giá/giờ phải lớn hơn 0");
                }

                if (Vehicle.PricePerDay <= 0)
                {
                    ModelState.AddModelError("Vehicle.PricePerDay", "Giá/ngày phải lớn hơn 0");
                }

                if (Vehicle.seartCapacity <= 0)
                {
                    ModelState.AddModelError("Vehicle.seartCapacity", "Chỗ ngồi phải lớn hơn 0");
                }

                if (Vehicle.MaxDistance < 0)
                {
                    ModelState.AddModelError("Vehicle.MaxDistance", "Quãng đường tối đa không được âm");
                }

                if (!ModelState.IsValid)
                {
                    System.Diagnostics.Debug.WriteLine($"ModelState Invalid. Errors:");
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            System.Diagnostics.Debug.WriteLine($"  - {error.ErrorMessage}");
                        }
                    }
                    // Reload vehicle from database but preserve user-entered values for editable fields
                    var currentImageUrl = Vehicle.ImageUrl;
                    var currentFeatures = Vehicle.Features;
                    Vehicle = await _vehicleService.GetVehicleByIdAsync(Vehicle.Id);
                    // Restore user-entered values
                    Vehicle.ImageUrl = currentImageUrl;
                    Vehicle.Features = currentFeatures;
                    return Page();
                }

                // Cập nhật xe
                System.Diagnostics.Debug.WriteLine($"Calling UpdateVehicleAsync for vehicle ID: {Vehicle.Id}");
                await _vehicleService.UpdateVehicleAsync(Vehicle);
                System.Diagnostics.Debug.WriteLine($"Vehicle updated successfully");
                
                TempData["SuccessMessage"] = "Cập nhật xe thành công!";
                return RedirectToPage("/Admin/Vehicle/Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in OnPostAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                TempData["ErrorMessage"] = $"Lỗi khi cập nhật xe: {ex.Message}";
                try
                {
                    var currentImageUrl = Vehicle.ImageUrl;
                    var currentFeatures = Vehicle.Features;
                    Stations = (await _stationService.GetAllStationsAsync()).ToList();
                    // Reload vehicle from database to show original data
                    Vehicle = await _vehicleService.GetVehicleByIdAsync(Vehicle.Id);
                    // Restore user-entered values
                    Vehicle.ImageUrl = currentImageUrl;
                    Vehicle.Features = currentFeatures;
                }
                catch { }
                return Page();
            }
        }
    }
}
