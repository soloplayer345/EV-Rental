using BusinessLayer.Services;
using DataAccessLayer.Entities;
using EV_Rental.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Admin.Station
{
    public class IndexModel : PageModel
    {
        private readonly StationService _stationService;
        public string UserEmail { get; set; } = string.Empty;
        public List<DataAccessLayer.Entities.Station> Stations { get; set; } = new();
        public string SearchQuery { get; set; } = "";
        public string FilterState { get; set; } = "";

        public IndexModel(StationService stationService)
        {
            _stationService = stationService;
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
                    Stations = Stations.Where(s =>
                        s.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        s.Address.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        s.State.Contains(search, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }

                // Lọc theo tỉnh/thành phố
                if (!string.IsNullOrEmpty(state))
                {
                    Stations = Stations.Where(s =>
                        s.State.Equals(state, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
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
    }
}
