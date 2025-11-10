using BusinessLayer.Services;
using DataAccessLayer.Entities;
using EV_Rental.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Admin.Station
{
    public class CreateModel : PageModel
    {
        private readonly StationService _stationService;
        public string UserEmail { get; set; } = string.Empty;

        [BindProperty]
        public DataAccessLayer.Entities.Station Station { get; set; } = new();

        public CreateModel(StationService stationService)
        {
            _stationService = stationService;
        }

        public IActionResult OnGet()
        {
            // Kiểm tra quyền Admin
            if (!SessionHelper.IsAdmin(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            var user = SessionHelper.GetUserSession(HttpContext.Session);
            UserEmail = user?.Email ?? "";

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
                return Page();
            }

            try
            {
                // Validate data
                if (string.IsNullOrWhiteSpace(Station.Name))
                {
                    ModelState.AddModelError("Station.Name", "Tên trạm không được trống");
                    return Page();
                }

                if (string.IsNullOrWhiteSpace(Station.Address))
                {
                    ModelState.AddModelError("Station.Address", "Địa chỉ không được trống");
                    return Page();
                }

                if (string.IsNullOrWhiteSpace(Station.State))
                {
                    ModelState.AddModelError("Station.State", "Tỉnh/Thành phố không được trống");
                    return Page();
                }

                await _stationService.AddStationAsync(Station);
                TempData["SuccessMessage"] = "Thêm trạm thành công!";
                return RedirectToPage("/Admin/Station/Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi thêm trạm: {ex.Message}";
                return Page();
            }
        }
    }
}
