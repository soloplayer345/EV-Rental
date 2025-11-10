using BusinessLayer.DTOs;
using BusinessLayer.Services;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using DataAccessLayer.Interfaces;
using EV_Rental.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Admin.Vehicle
{
    public class EditModel : PageModel
    {
        private readonly VehicleService _vehicleService;
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public DataAccessLayer.Entities.Vehicle Vehicle { get; set; } = new();

        public List<DataAccessLayer.Entities.Station> Stations { get; set; } = new();
        public string UserEmail { get; set; } = string.Empty;

        public EditModel(VehicleService vehicleService, IUnitOfWork unitOfWork)
        {
            _vehicleService = vehicleService;
            _unitOfWork = unitOfWork;
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
                var stationRepo = _unitOfWork.GetRepository<DataAccessLayer.Entities.Station>();
                Stations = (await stationRepo.GetAllAsync()).ToList();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToPage("/Admin/Vehicles");
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
                    var stationRepo = _unitOfWork.GetRepository<DataAccessLayer.Entities.Station>();
                    Stations = (await stationRepo.GetAllAsync()).ToList();
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

                if (Vehicle.PricePerHour <= 0 || Vehicle.PricePerDay <= 0)
                {
                    ModelState.AddModelError("", "Giá phải lớn hơn 0");
                    return Page();
                }

                await _vehicleService.UpdateVehicleAsync(Vehicle);
                TempData["SuccessMessage"] = "Cập nhật xe thành công!";
                return RedirectToPage("/Admin/Vehicle/Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi cập nhật xe: {ex.Message}";
                try
                {
                    var stationRepo = _unitOfWork.GetRepository<DataAccessLayer.Entities.Station>();
                    Stations = (await stationRepo.GetAllAsync()).ToList();
                }
                catch { }
                return Page();
            }
        }
    }
}
