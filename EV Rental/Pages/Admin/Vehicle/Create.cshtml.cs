using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.Entities;
using EV_Rental.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Admin.Vehicle
{
    public class CreateModel : PageModel
    {
        private readonly IVehicleService _vehicleService;
        private readonly IStationService _stationService;

        [BindProperty]
        public VehicleCreateDto Vehicle { get; set; } = new();

        public List<StationDto> Stations { get; set; } = new();
        public string UserEmail { get; set; } = string.Empty;

        public CreateModel(IVehicleService vehicleService, IStationService stationService)
        {
            _vehicleService = vehicleService;
            _stationService = stationService;
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

            try
            {
                // Lấy danh sách trạm
                Stations = (await _stationService.GetAllStationsAsync()).ToList();

                // Set default status
                Vehicle.Status = VehicleStatus.Available;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
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

            if (!ModelState.IsValid)
            {
                try
                {
                    Stations = (await _stationService.GetAllStationsAsync()).ToList();
                }
                catch { }
                return Page();
            }

            try
            {
                // Validate data
                if (string.IsNullOrWhiteSpace(Vehicle.Name))
                {
                    ModelState.AddModelError("Vehicle.Name", "Tên xe không được trống");
                    return Page();
                }

                if (string.IsNullOrWhiteSpace(Vehicle.Brand))
                {
                    ModelState.AddModelError("Vehicle.Brand", "Hãng không được trống");
                    return Page();
                }

                if (string.IsNullOrWhiteSpace(Vehicle.PlateNumber))
                {
                    ModelState.AddModelError("Vehicle.PlateNumber", "Biển số không được trống");
                    return Page();
                }

                if (Vehicle.StationId <= 0)
                {
                    ModelState.AddModelError("Vehicle.StationId", "Vui lòng chọn trạm");
                    return Page();
                }

                if (Vehicle.PricePerHour <= 0 || Vehicle.PricePerDay <= 0)
                {
                    ModelState.AddModelError("", "Giá phải lớn hơn 0");
                    return Page();
                }

                if (Vehicle.seartCapacity <= 0)
                {
                    ModelState.AddModelError("Vehicle.seartCapacity", "Chỗ ngồi phải lớn hơn 0");
                    return Page();
                }

                if (Vehicle.MaxDistance <= 0)
                {
                    ModelState.AddModelError("Vehicle.MaxDistance", "Quãng đường tối đa phải lớn hơn 0");
                    return Page();
                }

                // Set default features if empty
                if (string.IsNullOrWhiteSpace(Vehicle.Features))
                {
                    Vehicle.Features = "{}";
                }

                await _vehicleService.AddVehicleAsync(Vehicle);
                TempData["SuccessMessage"] = "Thêm xe thành công!";
                return RedirectToPage("/Admin/Vehicle/Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi thêm xe: {ex.Message}";
                try
                {
                    Stations = (await _stationService.GetAllStationsAsync()).ToList();
                }
                catch { }
                return Page();
            }
        }
    }
}

