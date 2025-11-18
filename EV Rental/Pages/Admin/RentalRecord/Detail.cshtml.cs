using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLayer.Interfaces;
using BusinessLayer.DTOs;
using BusinessLayer.Mapping;


namespace EV_Rental.Pages.Admin.RentalRecord
{
    public class DetailModel : PageModel
    {
        private readonly IRentalRecordService _rentalRecordService;

        public DetailModel(IRentalRecordService rentalRecordService)
        {
            _rentalRecordService = rentalRecordService;
        }

        public RentalRecordDto? RentalRecord { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Kiểm tra user đã đăng nhập chưa
            var userRole = HttpContext.Session.GetString("Role");
            var currentUserId = HttpContext.Session.GetInt32("AccountId");
            
            Console.WriteLine($"=== Detail Page Debug ===");
            Console.WriteLine($"ID parameter: {id}");
            Console.WriteLine($"User Role: {userRole}");
            Console.WriteLine($"User ID: {currentUserId}");
            Console.WriteLine($"Session available: {HttpContext.Session.IsAvailable}");
            Console.WriteLine($"All Session Keys:");
            foreach (var key in HttpContext.Session.Keys)
            {
                Console.WriteLine($"  - {key}");
            }
            
            // TEMPORARY: Comment out session check for debugging
            /*
            if (string.IsNullOrEmpty(userRole) || currentUserId == null)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để xem thông tin này!";
                return RedirectToPage("/Account/Login");
            }
            */

            try
            {
                RentalRecord = await _rentalRecordService.GetRentalRecordByIdAsync(id);
                Console.WriteLine($"RentalRecord found: {RentalRecord != null}");
                if (RentalRecord == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đơn thuê!";
                    
                    if (userRole == "Admin")
                        return RedirectToPage("Index");
                    else if (userRole == "Staff")
                        return RedirectToPage("/Staff/Dashboard");
                    else
                        return RedirectToPage("/Renter/MyTrips");
                }

                // Nếu là renter thì chỉ xem được đơn của mình
                if (userRole == "Renter" && RentalRecord.RenterId != currentUserId)
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền xem đơn thuê này!";
                    return RedirectToPage("/Renter/MyTrips");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                
                if (userRole == "Admin")
                    return RedirectToPage("Index");
                else if (userRole == "Staff")
                    return RedirectToPage("/Staff/Dashboard");
                else
                    return RedirectToPage("/Renter/MyTrips");
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

                record.Status = DataAccessLayer.Entities.RentalRecordStatus.Cancelled;
                await _rentalRecordService.UpdateRentalRecordAsync(RentalRecordMapper.ToEntity(record));

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

                record.Status = DataAccessLayer.Entities.RentalRecordStatus.Completed;
                record.ActualEndTime = DateTime.Now;
                await _rentalRecordService.UpdateRentalRecordAsync(RentalRecordMapper.ToEntity(record));

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





