using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using EV_Rental.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Admin.Station
{
    public class IndexModel : PageModel
    {
        private readonly IStationService _stationService;
        private readonly IVehicleService _vehicleService;
        public string UserEmail { get; set; } = string.Empty;
        public List<StationDto> Stations { get; set; } = new();
        public string SearchQuery { get; set; } = "";
        public string FilterState { get; set; } = "";

        public IndexModel(IStationService stationService, IVehicleService vehicleService)
        {
            _stationService = stationService;
            _vehicleService = vehicleService;
        }

        public async Task<IActionResult> OnGetAsync(string search = "", string state = "")
        {
            // Kiểm tra quyền Admin
            if (!SessionHelper.IsAdmin(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            var user = SessionHelper.GetUserSession(HttpContext.Session);
            UserEmail = user?.Email ?? "";

            SearchQuery = search;
            FilterState = state;

            try
            {
                // Lấy tất cả trạm
                var stations = await _stationService.GetAllStationsAsync();
                Stations = stations.ToList();

                // Lọc theo tìm kiếm
                if (!string.IsNullOrEmpty(search))
                {
                    Stations = Stations
                        .Where(s =>
                            s.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                            || s.Address.Contains(search, StringComparison.OrdinalIgnoreCase)
                            || s.State.Contains(search, StringComparison.OrdinalIgnoreCase)
                        )
                        .ToList();
                }

                // Lọc theo tỉnh/thành phố
                if (!string.IsNullOrEmpty(state))
                {
                    Stations = Stations
                        .Where(s => s.State.Equals(state, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                // Sắp xếp theo ngày tạo (mới nhất trước)
                Stations = Stations.OrderByDescending(s => s.Id).ToList();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tải dữ liệu trạm: {ex.Message}";
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
                await _stationService.DeleteStationAsync(id);
                TempData["SuccessMessage"] = "Xóa trạm thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi xóa trạm: {ex.Message}";
            }

            return RedirectToPage();
        }

        // API handlers for vehicle management modal
        public async Task<IActionResult> OnGetStationVehiclesAsync(int stationId)
        {
            if (!SessionHelper.IsAdmin(HttpContext.Session))
            {
                return Unauthorized();
            }

            try
            {
                var vehicles = await _vehicleService.GetVehiclesByStationIdAsync(stationId);
                var result = vehicles.Select(v => new
                {
                    id = v.Id,
                    name = v.Name,
                    plateNumber = v.PlateNumber,
                    vehicleType = v.VehicleType,
                    status = v.Status.ToString(),
                });
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = ex.Message }) { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> OnGetAvailableVehiclesAsync(
            int excludeStationId,
            int? sourceStationId = null
        )
        {
            if (!SessionHelper.IsAdmin(HttpContext.Session))
            {
                return Unauthorized();
            }

            try
            {
                var allVehicles = await _vehicleService.GetAllVehiclesAsync();

                // Filter: exclude current station, only Available/Maintenance status
                var filtered = allVehicles.Where(v =>
                    v.StationId != excludeStationId
                    && (
                        v.Status == VehicleStatus.Available || v.Status == VehicleStatus.Maintenance
                    )
                );

                // Optional: filter by source station
                if (sourceStationId.HasValue)
                {
                    filtered = filtered.Where(v => v.StationId == sourceStationId.Value);
                }

                var result = filtered.Select(v => new
                {
                    id = v.Id,
                    name = v.Name,
                    plateNumber = v.PlateNumber,
                    vehicleType = v.VehicleType,
                    status = v.Status.ToString(),
                    stationId = v.StationId,
                    stationName = v.Station?.Name ?? "N/A",
                });

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = ex.Message }) { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> OnGetAllStationsAsync()
        {
            if (!SessionHelper.IsAdmin(HttpContext.Session))
            {
                return Unauthorized();
            }

            try
            {
                var stations = await _stationService.GetAllStationsAsync();
                var result = stations.Select(s => new
                {
                    id = s.Id,
                    name = s.Name,
                    vehicleCount = s.Vehicles?.Count ?? 0,
                });
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = ex.Message }) { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> OnPostMoveVehicleAsync(
            [FromBody] MoveVehicleRequest request
        )
        {
            if (!SessionHelper.IsAdmin(HttpContext.Session))
            {
                return Unauthorized();
            }

            try
            {
                var vehicle = await _vehicleService.GetVehicleByIdAsync(request.VehicleId);
                if (vehicle == null)
                {
                    return new JsonResult(new { success = false, message = "Xe không tồn tại" });
                }

                // Check if vehicle is busy (đã cho thuê hoặc đang chờ khách nhận)
                if (
                    vehicle.Status == VehicleStatus.Rented
                    || vehicle.Status == VehicleStatus.WaitingForPickup
                )
                {
                    return new JsonResult(
                        new { success = false, message = "Không thể điều chuyển xe đang bận" }
                    );
                }

                // Update station
                vehicle.StationId = request.TargetStationId;
                await _vehicleService.UpdateVehicleAsync(vehicle);

                return new JsonResult(
                    new { success = true, message = "Điều chuyển xe thành công" }
                );
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message })
                {
                    StatusCode = 500,
                };
            }
        }

        public class MoveVehicleRequest
        {
            public int VehicleId { get; set; }
            public int TargetStationId { get; set; }
        }
    }
}
