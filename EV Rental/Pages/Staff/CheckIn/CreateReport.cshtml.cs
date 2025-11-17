using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using EV_Rental.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages.Staff.CheckIn
{
    public class CreateReportModel : PageModel
    {
        private readonly ICheckInService _checkInService;

        public CreateReportModel(ICheckInService checkInService)
        {
            _checkInService = checkInService;
        }

        public RentalRecordDto? RentalRecord { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
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
                RentalRecord = await _checkInService.GetBillingAsync(id);
                
                if (RentalRecord == null)
                {
                    TempData["ErrorMessage"] = $"Không tìm thấy thông tin thuê xe với ID: {id}";
                    return RedirectToPage("./Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tải dữ liệu: {ex.Message}";
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
