using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLayer.Services;
using DataAccessLayer.Entities;

namespace EV_Rental.Pages.Admin.RentalRecord
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
            // Check admin permission
            var adminRole = HttpContext.Session.GetString("Role");
            if (adminRole != "Admin")
            {
                return Redirect("/");
            }

            try
            {
                RentalRecord = await _rentalRecordService.GetRentalRecordByIdAsync(id);
                if (RentalRecord == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đơn thuê!";
                    return RedirectToPage("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToPage("Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            // Check admin permission
            var adminRole = HttpContext.Session.GetString("Role");
            if (adminRole != "Admin")
            {
                return Redirect("/");
            }

            try
            {
                var record = await _rentalRecordService.GetRentalRecordByIdAsync(id);
                if (record == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đơn thuê!";
                    return RedirectToPage("Index");
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

        public async Task<IActionResult> OnPostCompleteAsync(int id)
        {
            // Check admin permission
            var adminRole = HttpContext.Session.GetString("Role");
            if (adminRole != "Admin")
            {
                return Redirect("/");
            }

            try
            {
                var record = await _rentalRecordService.GetRentalRecordByIdAsync(id);
                if (record == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đơn thuê!";
                    return RedirectToPage("Index");
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
    }
}
