using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using EV_Rental.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Staff.CheckIn
{
    public class IndexModel : PageModel
    {
        private readonly ICheckInService _checkInService;

        public IndexModel(ICheckInService checkInService)
        {
            _checkInService = checkInService;
        }

        public List<RentalRecordDto> ActiveRentals { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            // Kiểm tra authentication
            if (!SessionHelper.IsLoggedIn(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            var user = SessionHelper.GetUserSession(HttpContext.Session);
            if (user?.Role != DataAccessLayer.Entities.AccountRole.Staff)
            {
                return RedirectToPage("/Index");
            }

            try
            {
                // Load danh sách đơn thuê đang hoạt động
                ActiveRentals = await _checkInService.GetActiveRentalsAsync();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tải dữ liệu: {ex.Message}";
            }

            return Page();
        }
    }
}
