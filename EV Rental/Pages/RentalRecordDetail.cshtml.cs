using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using BusinessLayer.Mapping;
using EV_Rental.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EV_Rental.Pages
{
    public class RentalRecordDetailModel : PageModel
    {
        private readonly IRentalRecordService _rentalRecordService;

        public RentalRecordDetailModel(IRentalRecordService rentalRecordService)
        {
            _rentalRecordService = rentalRecordService;
        }

        public RentalRecordDto? RentalRecord { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Debug: Log session info
            var accountId = SessionHelper.GetAccountId(HttpContext);
            var userRole = SessionHelper.GetUserRole(HttpContext.Session);

            Console.WriteLine($"=== RentalRecordDetail Debug ===");
            Console.WriteLine($"ID parameter: {id}");
            Console.WriteLine($"AccountId from session: {accountId}");
            Console.WriteLine($"Role from session: {userRole}");

            // Check if user is logged in
            if (accountId == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để xem chi tiết đơn thuê!";
                return RedirectToPage("/Account/Login");
            }

            // Check admin permission
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] =
                    "Bạn không có quyền truy cập trang này! (Role hiện tại: "
                    + (userRole ?? "null")
                    + ")";
                return RedirectToPage("/Admin/RentalRecord/Index");
            }

            try
            {
                RentalRecord = await _rentalRecordService.GetRentalRecordByIdAsync(id);
                if (RentalRecord == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đơn thuê!";
                    return RedirectToPage("/Admin/RentalRecord/Index");
                }

                Console.WriteLine($"RentalRecord loaded successfully: ID={RentalRecord.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading rental record: {ex.Message}");
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToPage("/Admin/RentalRecord/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            // Check admin permission
            var userRole = SessionHelper.GetUserRole(HttpContext.Session);
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "Bạn không có quyền thực hiện hành động này!";
                return Redirect("/");
            }

            try
            {
                var record = await _rentalRecordService.GetRentalRecordByIdAsync(id);
                if (record == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đơn thuê!";
                    return RedirectToPage("/Admin/RentalRecord/Index");
                }

                record.Status = RentalRecordStatus.Cancelled;
                await _rentalRecordService.UpdateRentalRecordAsync(
                    RentalRecordMapper.ToEntity(record)
                );

                TempData["SuccessMessage"] = "Hủy đơn thuê thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi hủy đơn thuê: {ex.Message}";
            }

            return RedirectToPage("RentalRecordDetail", new { id });
        }

        public async Task<IActionResult> OnPostCompleteAsync(int id)
        {
            // Check admin permission
            var userRole = SessionHelper.GetUserRole(HttpContext.Session);
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "Bạn không có quyền thực hiện hành động này!";
                return Redirect("/");
            }

            try
            {
                var record = await _rentalRecordService.GetRentalRecordByIdAsync(id);
                if (record == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đơn thuê!";
                    return RedirectToPage("/Admin/RentalRecord/Index");
                }

                record.Status = RentalRecordStatus.Completed;
                record.ActualEndTime = DateTime.Now;
                await _rentalRecordService.UpdateRentalRecordAsync(
                    RentalRecordMapper.ToEntity(record)
                );

                TempData["SuccessMessage"] = "Đánh dấu đơn thuê hoàn thành!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToPage("RentalRecordDetail", new { id });
        }
    }
}
