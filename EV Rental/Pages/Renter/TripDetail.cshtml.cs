using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLayer.Services;
using EV_Rental.Helpers;

namespace EV_Rental.Pages.Renter
{
    public class TripDetailModel : PageModel
    {
        private readonly RentalRecordService _rentalRecordService;

        public TripDetailModel(RentalRecordService rentalRecordService)
        {
            _rentalRecordService = rentalRecordService;
        }

        public DataAccessLayer.Entities.RentalRecord? RentalRecord { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Check renter permission
            var userRole = SessionHelper.GetUserRole(HttpContext.Session);
            var accountId = SessionHelper.GetAccountId(HttpContext);
            
            if (accountId == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập!";
                return RedirectToPage("/Account/Login");
            }
            
            if (userRole != "Renter")
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập trang này!";
                return RedirectToPage("/Renter/Index");
            }

            try
            {
                RentalRecord = await _rentalRecordService.GetRentalRecordByIdAsync(id);
                if (RentalRecord == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy chuyến đi!";
                    return RedirectToPage("/Renter/MyTrips");
                }

                // Chỉ cho phép xem đơn của chính mình
                if (RentalRecord.RenterId != accountId)
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền xem chuyến đi này!";
                    return RedirectToPage("/Renter/MyTrips");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToPage("/Renter/MyTrips");
            }

            return Page();
        }
    }
}
