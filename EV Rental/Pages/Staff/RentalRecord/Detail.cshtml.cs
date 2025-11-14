using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLayer.Services;
using EV_Rental.Helpers;

namespace EV_Rental.Pages.Staff.RentalRecord
{
    public class DetailModel : PageModel
    {
        private readonly RentalRecordService _rentalRecordService;

        public DetailModel(RentalRecordService rentalRecordService)
        {
            _rentalRecordService = rentalRecordService;
        }

        public DataAccessLayer.Entities.RentalRecord? RentalRecord { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Check staff permission
            var userRole = SessionHelper.GetUserRole(HttpContext.Session);
            var accountId = SessionHelper.GetAccountId(HttpContext);
            
            if (accountId == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập!";
                return RedirectToPage("/Account/Login");
            }
            
            if (userRole != "Staff")
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập trang này!";
                return RedirectToPage("/Staff/Dashboard");
            }

            try
            {
                RentalRecord = await _rentalRecordService.GetRentalRecordByIdAsync(id);
                if (RentalRecord == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đơn thuê!";
                    return RedirectToPage("/Staff/Dashboard");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToPage("/Staff/Dashboard");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostCompleteAsync(int id)
        {
            // Check staff permission
            var userRole = SessionHelper.GetUserRole(HttpContext.Session);
            if (userRole != "Staff")
            {
                TempData["ErrorMessage"] = "Bạn không có quyền thực hiện hành động này!";
                return RedirectToPage("/Staff/Dashboard");
            }

            try
            {
                var record = await _rentalRecordService.GetRentalRecordByIdAsync(id);
                if (record == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đơn thuê!";
                    return RedirectToPage("/Staff/Dashboard");
                }

                record.Status = DataAccessLayer.Enums.RentalRecordStatus.Completed;
                record.ActualEndTime = DateTime.Now;
                await _rentalRecordService.UpdateRentalRecordAsync(record);

                TempData["SuccessMessage"] = "Đánh dấu đơn thuê hoàn thành!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToPage("Detail", new { id });
        }

        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            // Check staff permission
            var userRole = SessionHelper.GetUserRole(HttpContext.Session);
            if (userRole != "Staff")
            {
                TempData["ErrorMessage"] = "Bạn không có quyền thực hiện hành động này!";
                return RedirectToPage("/Staff/Dashboard");
            }

            try
            {
                var record = await _rentalRecordService.GetRentalRecordByIdAsync(id);
                if (record == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đơn thuê!";
                    return RedirectToPage("/Staff/Dashboard");
                }

                record.Status = DataAccessLayer.Enums.RentalRecordStatus.Cancelled;
                await _rentalRecordService.UpdateRentalRecordAsync(record);

                TempData["SuccessMessage"] = "Hủy đơn thuê thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi hủy đơn thuê: {ex.Message}";
            }

            return RedirectToPage("Detail", new { id });
        }
    }
}
