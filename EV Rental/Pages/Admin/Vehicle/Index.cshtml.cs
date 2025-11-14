using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.Enums;
using EV_Rental.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Admin.Vehicle
{
    public class IndexModel : PageModel
    {
        private readonly IVehicleService _vehicleService;
        public string UserEmail { get; set; } = string.Empty;
        public List<VehicleDto> Vehicles { get; set; } = new();
        public string SearchQuery { get; set; } = "";
        public string FilterStatus { get; set; } = "";
        public string FilterType { get; set; } = "";

        public IndexModel(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        public async Task<IActionResult> OnGetAsync(string search = "", string status = "", string type = "")
        {
            // Kiểm tra quyền Admin
            if (!SessionHelper.IsAdmin(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            var user = SessionHelper.GetUserSession(HttpContext.Session);
            UserEmail = user?.Email ?? "";

            SearchQuery = search;
            FilterStatus = status;
            FilterType = type;

            try
            {
                // Lấy tất cả xe
                var vehicles = await _vehicleService.GetVehiclesAsync();
                Vehicles = vehicles.ToList();

                // Lọc theo tìm kiếm
                if (!string.IsNullOrEmpty(search))
                {
                    Vehicles = Vehicles.Where(v =>
                        v.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        v.Brand.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        v.PlateNumber.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        v.Model.Contains(search, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }

                // Lọc theo trạng thái
                if (!string.IsNullOrEmpty(status) && Enum.TryParse<VehicleStatus>(status, out var statusEnum))
                {
                    Vehicles = Vehicles.Where(v => v.Status == statusEnum).ToList();
                }

                // Lọc theo loại xe
                if (!string.IsNullOrEmpty(type))
                {
                    Vehicles = Vehicles.Where(v =>
                        v.VehicleType.Equals(type, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }

                // Sắp xếp theo ngày tạo (mới nhất trước)
                Vehicles = Vehicles.OrderByDescending(v => v.Id).ToList();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tải dữ liệu xe: {ex.Message}";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            // Kiểm tra quyền Admin
            if (!SessionHelper.IsAdmin(HttpContext.Session))
            {
                return Unauthorized();
            }

            try
            {
                await _vehicleService.DeleteVehicleAsync(id);
                TempData["SuccessMessage"] = "Xóa xe thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi xóa xe: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAsync(int[] ids)
        {
            // Kiểm tra quyền Admin
            if (!SessionHelper.IsAdmin(HttpContext.Session))
            {
                return Unauthorized();
            }

            if (ids == null || ids.Length == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn ít nhất một xe để xóa!";
                return RedirectToPage();
            }

            try
            {
                int deletedCount = 0;
                var failedVehicles = new List<string>();

                foreach (var id in ids)
                {
                    try
                    {
                        await _vehicleService.DeleteVehicleAsync(id);
                        deletedCount++;
                    }
                    catch (Exception ex)
                    {
                        var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
                        failedVehicles.Add(vehicle?.Name ?? $"ID: {id}");
                        System.Diagnostics.Debug.WriteLine($"Lỗi xóa xe ID {id}: {ex.Message}");
                    }
                }

                if (deletedCount > 0)
                {
                    if (failedVehicles.Count > 0)
                    {
                        TempData["WarningMessage"] = $"Đã xóa {deletedCount} xe thành công, nhưng không thể xóa {failedVehicles.Count} xe: {string.Join(", ", failedVehicles)}";
                    }
                    else
                    {
                        TempData["SuccessMessage"] = $"Đã xóa {deletedCount} xe thành công!";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể xóa các xe được chọn!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi xóa xe: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}

